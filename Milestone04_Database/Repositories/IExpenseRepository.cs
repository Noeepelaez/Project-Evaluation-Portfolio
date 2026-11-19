using Milestone04_Database.Models;

namespace Milestone04_Database.Repositories;

public interface IExpenseRepository
{
    List<Expense> GetByUser(int userId);
    Expense? GetById(int id);
    List<Expense> Search(int userId, string text);
    List<Expense> GetByMonth(int userId, int year, int month);
    decimal GetMonthTotal(int userId, int year, int month);
    List<CategoryTotal> GetTotalsByCategory(int userId, int year, int month);
    List<DateTime> GetMonthsWithExpenses(int userId);
    void Add(Expense expense);
    void Update(Expense expense);
    void Delete(int id, int userId);
}
