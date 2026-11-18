using Milestone03_LayeredConsole.Models;

namespace Milestone03_LayeredConsole.Repositories;

public class JsonExpenseRepository : IExpenseRepository
{
    private readonly JsonFileStore<Expense> _store;
    private readonly List<Expense> _expenses;

    public JsonExpenseRepository(string filePath)
    {
        _store = new JsonFileStore<Expense>(filePath);
        _expenses = _store.Load();
    }

    public List<Expense> GetByUser(int userId)
    {
        return _expenses.Where(e => e.UserId == userId).ToList();
    }

    public Expense? GetById(int id)
    {
        return _expenses.FirstOrDefault(e => e.Id == id);
    }

    public void Add(Expense expense)
    {
        expense.Id = _expenses.Count == 0 ? 1 : _expenses.Max(e => e.Id) + 1;
        _expenses.Add(expense);
        _store.Save(_expenses);
    }

    public void Update(Expense expense)
    {
        int index = _expenses.FindIndex(e => e.Id == expense.Id);
        if (index >= 0)
        {
            _expenses[index] = expense;
            _store.Save(_expenses);
        }
    }

    public void Delete(int id)
    {
        _expenses.RemoveAll(e => e.Id == id);
        _store.Save(_expenses);
    }
}
