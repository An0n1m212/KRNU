using TestingSystem.Models;

namespace TestingSystem.Abstractions;

public interface IGroupRepository
{
    IReadOnlyList<Group> GetAll();
    Group? GetById(int id);
    /// <summary>Создаёт (Id == 0) или обновляет группу.</summary>
    Group Save(Group group);
    void Delete(int id);
}
