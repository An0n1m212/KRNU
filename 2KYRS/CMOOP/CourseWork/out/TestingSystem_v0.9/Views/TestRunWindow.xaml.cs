using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>
/// Экран прохождения теста. Логику сессии и таймера ведёт TestSessionController, нарушения — AntiCheatMonitor;
/// окно только отображает состояние и передаёт действия пользователя.
/// </summary>
public partial class TestRunWindow : Window
{
    private const int MaxViolations = 5;

    private readonly AppServices _s;
    private readonly TestSessionController _c;
    private readonly AntiCheatMonitor _anti;
    private readonly IDisposable _idleSuspension;      // на время теста блокировка по бездействию не действует
    private IQuestionView? _view;
    private int _index = -1;

    public TestRunWindow(AppServices services, TestSessionController controller)
    {
        InitializeComponent();
        _s = services;
        _c = controller;

        // У теста свой таймер и античит; экран блокировки (отдельное окно) считался бы нарушением и мешал бы сдаче
        _idleSuspension = _s.Inactivity.Suspend();

        Title = _c.Test.Title;
        TestTitleText.Text = _c.Test.Title;
        AnsweredBar.Maximum = _c.Questions.Count;

        // --- античит ---
        _anti = new AntiCheatMonitor(this, MaxViolations);
        _anti.ViolationRegistered += OnViolation;
        _anti.LimitReached += (_, _) =>
        {
            ShowBanner(_s.Localization["Run_LimitReached"]);
            _c.Submit();                                  // слишком много нарушений -> автоматическая сдача
        };

        // --- таймер и состояния сессии ---
        _c.Tick += (_, _) => UpdateTimer();
        _c.TimeWarning += (_, _) => ShowBanner(_s.Localization["Run_TimeWarning"]);
        _c.BeforeComplete += (_, _) => SaveCurrent();     // сохраняем ответ на текущий вопрос перед любым завершением
        _c.Finished += (_, result) => OnFinished(result);

        Loaded += (_, _) =>
        {
            _c.Begin();                                   // InProgress, дедлайн, запуск таймера
            UpdateTimer();
            ShowQuestion(0);
        };
    }

    // ---------------------------------------------------------------- таймер

    private void UpdateTimer()
    {
        if (_c.Remaining is not { } left)
        {
            TimerText.Text = "∞";
            return;
        }

        TimerText.Text = $"{(int)left.TotalMinutes:00}:{left.Seconds:00}";
        if (left.TotalSeconds <= TestSessionController.WarningSeconds)
            TimerText.SetResourceReference(TextBlock.ForegroundProperty, "ErrorBrush");   // последняя минута — красным
        else
            TimerText.ClearValue(TextBlock.ForegroundProperty);
    }

    private void ShowBanner(string text)
    {
        BannerText.Text = text;
        Banner.Visibility = Visibility.Visible;
    }

    // ---------------------------------------------------------------- античит

    private void OnViolation(object? sender, ViolationEventArgs e)
    {
        _c.RegisterViolation();
        var loc = _s.Localization;
        ViolationsText.Text = loc.Format("Run_Violations", e.Count, MaxViolations);
        ShowBanner(loc.Format(e.Kind == ViolationKind.Minimized ? "Run_ViolationMinimized" : "Run_ViolationFocus", e.Count, MaxViolations));
    }

    // ---------------------------------------------------------------- вопросы

    private void SaveCurrent()
    {
        if (_view != null && !_c.IsFinished) _c.SaveAnswer(_view.Read());
    }

    private void ShowQuestion(int index)
    {
        SaveCurrent();
        _index = index;

        var rq = _c.Questions[index];
        var q = rq.Source;
        var loc = _s.Localization;

        // ФАБРИКА: UI создаётся по типу вопроса
        _view = QuestionViewFactory.Create(rq, _anti, loc);
        _view.Load(_c.GetAnswer(q.Id));
        ContentHost.Content = _view.Element;

        QuestionHeader.Text = loc.Format("Run_QuestionOf", index + 1, _c.Questions.Count, q.MaxScore);
        QuestionText.Text = q.Text;
        HintText.Text = loc[$"Hint_{q.QuestionType}"];

        var image = ImageLoader.TryLoad(q.MediaPath);
        MediaImage.Source = image;
        MediaImage.Visibility = image is null ? Visibility.Collapsed : Visibility.Visible;

        PrevButton.IsEnabled = index > 0;
        NextButton.IsEnabled = index < _c.Questions.Count - 1;

        _c.SetCurrentIndex(index);
        RefreshNavigation();
    }

    private void RefreshNavigation()
    {
        NavPanel.Children.Clear();
        for (int i = 0; i < _c.Questions.Count; i++)
        {
            var button = new Button { Content = (i + 1).ToString(), Width = 38, Height = 32, Margin = new Thickness(3), Tag = i };
            if (_c.IsAnswered(_c.Questions[i].Source.Id)) button.SetResourceReference(BackgroundProperty, "AnsweredBrush");
            if (i == _index)
            {
                button.BorderThickness = new Thickness(2);
                button.SetResourceReference(BorderBrushProperty, "AccentBrush");
            }
            button.Click += (s, _) => ShowQuestion((int)((Button)s).Tag);
            NavPanel.Children.Add(button);
        }

        AnsweredBar.Value = _c.AnsweredCount;
        AnsweredText.Text = _s.Localization.Format("Run_Answered", _c.AnsweredCount, _c.Questions.Count);
    }

    private void Prev_Click(object sender, RoutedEventArgs e) { if (_index > 0) ShowQuestion(_index - 1); }
    private void Next_Click(object sender, RoutedEventArgs e) { if (_index < _c.Questions.Count - 1) ShowQuestion(_index + 1); }

    // ---------------------------------------------------------------- завершение

    private void Submit_Click(object sender, RoutedEventArgs e)
    {
        SaveCurrent();
        var loc = _s.Localization;
        int unanswered = _c.Questions.Count - _c.AnsweredCount;
        string message = unanswered > 0 ? loc.Format("Run_ConfirmSubmitUnanswered", unanswered) : loc["Run_ConfirmSubmit"];

        MessageBoxResult answer;
        using (_anti.Suspend())                           // собственный диалог — не нарушение
            answer = MessageBox.Show(this, message, loc["Msg_ConfirmTitle"], MessageBoxButton.YesNo);
        if (answer == MessageBoxResult.Yes) _c.Submit();
    }

    private void OnFinished(Result result)
    {
        var loc = _s.Localization;
        _anti.Dispose();

        bool expired = _c.Session.Status == ResultStatus.Expired;
        ResultTitle.Text = loc[expired ? "Run_TimeUp" : "Run_Done"];

        if (result.Status == ResultStatus.PendingTeacherReview)
            ResultBody.Text = loc.Format("Run_Pending", result.TotalScore, result.MaxScore);
        else
            ResultBody.Text = loc.Format(result.IsPassed ? "Run_Passed" : "Run_Failed", result.TotalScore, result.MaxScore, result.Percentage);

        RunGrid.Visibility = Visibility.Collapsed;
        ResultOverlay.Visibility = Visibility.Visible;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!_c.IsFinished)
        {
            var loc = _s.Localization;
            MessageBoxResult answer;
            using (_anti.Suspend())
                answer = MessageBox.Show(this, loc["Run_ConfirmLeave"], loc["Msg_ConfirmTitle"], MessageBoxButton.YesNo);

            if (answer != MessageBoxResult.Yes) { e.Cancel = true; return; }
            _c.Submit();                                  // выход = сдача того, что успел ответить
        }
        base.OnClosing(e);
    }

    protected override void OnClosed(EventArgs e)
    {
        _anti.Dispose();
        _idleSuspension.Dispose();                        // отсчёт бездействия возобновится с нуля
        base.OnClosed(e);
    }
}
