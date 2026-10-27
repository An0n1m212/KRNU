using TestingSystem.Models;

namespace TestingSystem.Abstractions;

public interface ILectureRepository
{
    IReadOnlyList<Lecture> GetAll();
    Lecture? GetById(int id);
    /// <summary>Створює (Id == 0) або оновлює лекцію.</summary>
    Lecture Save(Lecture lecture);
    void Delete(int id);
}
