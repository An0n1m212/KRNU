using System.Windows.Threading;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Services;

/// <summary>Вопрос в том виде, в каком его видит ученик (порядок вариантов уже перемешан, если нужно).</summary>
public sealed class RuntimeQuestion
{
    public Question Source { get; }
    /// <summary>Варианты в порядке показа. Для Matching — левые элементы по OrderIndex, правые перемешиваются во View.</summary>
    public IReadOnlyList<Answer> Answers { get; }

    public RuntimeQuestion(Question source, IReadOnlyList<Answer> answers)
    {
        Source = source;
        Answers = answers;
    }
}

/// <summary>
/// Управляет одной попыткой тестирования: таймер, ответы, нарушения и ПЕРЕХОДЫ СОСТОЯНИЙ сессии.
///
///   [создана] --Begin()--> InProgress
///   InProgress --Submit()--> Completed              (все ответы проверены автоматически)
///   InProgress --Submit()--> PendingTeacherReview   (есть ответы, требующие ручной проверки)
///   InProgress --таймер истёк (Expire())--> Expired (сессия); результат: Expired либо PendingTeacherReview
///   PendingTeacherReview --проверка учителем--> Completed (см. TestSessionService.CompleteReview)
///
/// Из конечных состояний назад вернуться нельзя: после Complete() все методы изменения состояния игнорируются.
/// </summary>
public sealed class TestSessionController
{
    public const int WarningSeconds = 60;

    private readonly IResultRepository _results;
    private readonly GradingEngine _grading;
    private readonly Dictionary<int, StudentAnswerDetail> _answers = new();
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private bool _warned;

    public TestSession Session { get; }
    public Test Test { get; }
    public IReadOnlyList<RuntimeQuestion> Questions { get; }
    public DateTime? Deadline { get; private set; }
    public Result? Result { get; private set; }

    public bool HasTimeLimit => Test.TimeLimitMinutes > 0;
    public bool IsFinished => Session.Status != ResultStatus.InProgress;

    /// <summary>Оставшееся время (null — без ограничения).</summary>
    public TimeSpan? Remaining
    {
        get
        {
            if (Deadline is null) return null;
            var left = Deadline.Value - DateTime.Now;
            return left < TimeSpan.Zero ? TimeSpan.Zero : left;
        }
    }

    public int AnsweredCount => _answers.Values.Count(AnswerState.HasAnswer);

    public event EventHandler? Tick;                       // каждую секунду
    public event EventHandler? TimeWarning;                // осталась последняя минута (один раз)
    public event EventHandler? BeforeComplete;             // перед подсчётом: View сохраняет текущий ответ
    public event EventHandler<Result>? Finished;           // попытка завершена (отправка или таймаут)

    public TestSessionController(TestSession session, Test test, IReadOnlyList<RuntimeQuestion> questions,
                                 IResultRepository results, GradingEngine grading)
    {
        Session = session;
        Test = test;
        Questions = questions;
        _results = results;
        _grading = grading;
    }

    /// <summary>Старт: фиксируем время, считаем дедлайн и запускаем таймер.</summary>
    public void Begin()
    {
        Session.StartTime = DateTime.Now;
        Session.Status = ResultStatus.InProgress;
        Deadline = HasTimeLimit ? Session.StartTime.AddMinutes(Test.TimeLimitMinutes) : null;
        _results.SaveSession(Session);

        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (IsFinished) return;

        Tick?.Invoke(this, EventArgs.Empty);

        if (Remaining is { } left)
        {
            if (!_warned && left.TotalSeconds <= WarningSeconds && left.TotalSeconds > 0)
            {
                _warned = true;
                TimeWarning?.Invoke(this, EventArgs.Empty);
            }
            if (left <= TimeSpan.Zero) Expire();           // время вышло -> автоматическая сдача
        }
    }

    public void SaveAnswer(StudentAnswerDetail detail)
    {
        if (IsFinished) return;
        _answers[detail.QuestionId] = detail.Clone();
    }

    public StudentAnswerDetail? GetAnswer(int questionId) => _answers.TryGetValue(questionId, out var d) ? d.Clone() : null;

    public bool IsAnswered(int questionId) => AnswerState.HasAnswer(GetAnswer(questionId));

    public void SetCurrentIndex(int index)
    {
        if (IsFinished) return;
        Session.CurrentQuestionIndex = index;
        _results.SaveSession(Session);
    }

    public void RegisterViolation()
    {
        if (IsFinished) return;
        Session.ViolationsCount++;
        _results.SaveSession(Session);
    }

    public Result Submit() => Complete(timedOut: false);
    public Result Expire() => Complete(timedOut: true);

    private Result Complete(bool timedOut)
    {
        if (IsFinished) return Result!;

        BeforeComplete?.Invoke(this, EventArgs.Empty);

        _timer.Stop();
        _timer.Tick -= OnTimerTick;

        Session.EndTime = DateTime.Now;
        var result = _grading.BuildResult(Session, Test, _answers, timedOut);

        // Сессия при таймауте остаётся Expired даже если часть ответов ждёт проверки учителем
        Session.Status = timedOut ? ResultStatus.Expired : result.Status;
        _results.SaveSession(Session);

        Result = _results.SaveResult(result);
        Finished?.Invoke(this, Result);
        return Result;
    }
}
