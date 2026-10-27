using TestingSystem.Models;

namespace TestingSystem.Abstractions;

public interface ICategoryRepository
{
    IReadOnlyList<Category> GetAll();
    Category? GetById(int id);
    Category Save(Category category);
    void Delete(int id);
}
