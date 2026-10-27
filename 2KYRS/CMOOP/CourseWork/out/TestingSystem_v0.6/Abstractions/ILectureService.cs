using TestingSystem.Models;

namespace TestingSystem.Abstractions;

/// <summary>Бізнес-логіка лекцій: хто що бачить і хто що може змінювати.</summary>
public interface ILectureService
{
    /// <summary>Лекції, якими може керувати поточний користувач (викладач — свої, адмін — усі).</summary>
    IReadOnlyList<Lecture> GetManageable();

    /// <summary>Опубліковані лекції, доступні групі поточного студента.</summary>
    IReadOnlyList<Lecture> GetForCurrentStudent();

    Lecture? GetById(int id);

    /// <summary>Бросає UnauthorizedAccessException, якщо у користувача немає прав.</summary>
    Lecture Save(Lecture lecture);
    void Delete(int id);
}
