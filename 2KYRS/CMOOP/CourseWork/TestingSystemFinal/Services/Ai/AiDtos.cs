namespace TestingSystem.Services.Ai;

// DTO для розбору JSON-відповідей AI. Усі колекції можуть прийти як null — перед використанням нормалізуються.

internal sealed class TopicsDto { public List<TopicDto>? Topics { get; set; } }
internal sealed class TopicDto { public string? Title { get; set; } public string? Description { get; set; } }

internal sealed class LectureDto
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public string? Content { get; set; }
}

internal sealed class TestDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<QuestionDto>? Questions { get; set; }
}

internal sealed class QuestionDto
{
    public string? Type { get; set; }
    public string? Text { get; set; }
    public string? Explanation { get; set; }
    public double? MaxScore { get; set; }
    public List<AnswerDto>? Answers { get; set; }              // Single / Multiple
    public List<string>? AcceptedAnswers { get; set; }         // TextAnswer
    public string? TextEvaluation { get; set; }                // TextAnswer
    public List<PairDto>? Pairs { get; set; }                  // Matching
    public List<string>? Items { get; set; }                   // Sequence (у правильному порядку)
}

internal sealed class AnswerDto { public string? Text { get; set; } public bool IsCorrect { get; set; } }
internal sealed class PairDto { public string? Left { get; set; } public string? Right { get; set; } }
internal sealed class GradeDto { public double Ratio { get; set; } public string? Comment { get; set; } }
