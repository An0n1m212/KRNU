namespace TestingSystem.Models;

/// <summary>
/// Лекція для груп. Текст зберігається у Markdown (заголовки, списки, **жирний**, `код`) —
/// відображається безпечним власним рендерером (без HTML та посилань).
/// </summary>
public class Lecture
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int AuthorTeacherId { get; set; }
    public int? CategoryId { get; set; }
    /// <summary>Групи, яким доступна лекція; порожньо — усім студентам.</summary>
    public List<int> AssignedGroupIds { get; set; } = new();
    /// <summary>false — чернетка (бачить лише автор/адмін).</summary>
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    /// <summary>Мітка: текст початково створено за допомогою AI (викладач його перевірив і зберіг).</summary>
    public bool IsAiGenerated { get; set; }
    /// <summary>Тест за темою лекції (підказка студенту, що пройти після читання).</summary>
    public int? RelatedTestId { get; set; }

    public Lecture DeepClone()
    {
        var c = (Lecture)MemberwiseClone();
        c.AssignedGroupIds = new List<int>(AssignedGroupIds);
        return c;
    }
}
