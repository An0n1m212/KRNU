using TestingSystem.Models;

namespace TestingSystem.Abstractions;

public interface ITestRepository
{
    IReadOnlyList<Test> GetAll();
    Test? GetById(int id);
    /// <summary>Создаёт (Id == 0) или обновляет тест с вопросами и ответами; присваивает Id и внешние ключи.</summary>
    Test Save(Test test);
    void Delete(int id);
}
