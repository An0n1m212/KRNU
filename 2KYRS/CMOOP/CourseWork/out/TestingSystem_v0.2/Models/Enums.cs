namespace TestingSystem.Models;

public enum Role { Admin, Teacher, Student }

public enum QuestionType { SingleChoice, MultipleChoice, TextAnswer, Matching, Sequence, FileUpload }

public enum TextEvaluationType { AutoExactMatch, AutoContains, ManualTeacherReview }

/// <summary>Статус сессии/результата (конечный автомат описан в TestSessionController).</summary>
public enum ResultStatus { InProgress, PendingTeacherReview, Completed, Expired }
