using Milestone03_LayeredConsole.Models;

namespace Milestone03_LayeredConsole.Repositories;

public interface IExpenseRepository
{
    List<Expense> GetByUser(int userId);
    Expense? GetById(int id);
    void Add(Expense expense);
    void Update(Expense expense);
    void Delete(int id);
}
