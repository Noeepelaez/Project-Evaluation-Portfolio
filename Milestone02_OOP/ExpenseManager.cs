using Milestone02_OOP.Models;

namespace Milestone02_OOP;

public class ExpenseManager
{
    private readonly List<Expense> _expenses;
    private readonly ExpenseFileStorage _storage;
    private int _nextId;

    public ExpenseManager(ExpenseFileStorage storage)
    {
        _storage = storage;
        _expenses = storage.Load();
        _nextId = _expenses.Count == 0 ? 1 : _expenses.Max(e => e.Id) + 1;
    }

    public int Count => _expenses.Count;

    public List<Expense> GetAll()
    {
        return _expenses.OrderBy(e => e.Date).ThenBy(e => e.Id).ToList();
    }

    public Expense Add(string description, decimal amount, Category category, DateTime date)
    {
        var expense = new Expense(_nextId, description, amount, category, date);
        _expenses.Add(expense);
        _nextId++;
        Save();
        return expense;
    }

    public Expense? FindById(int id)
    {
        return _expenses.FirstOrDefault(e => e.Id == id);
    }

    public List<Expense> Search(string text)
    {
        return GetAll().Where(e => e.Matches(text)).ToList();
    }

    public bool Update(int id, string description, decimal amount, Category category, DateTime date)
    {
        Expense? expense = FindById(id);
        if (expense == null)
        {
            return false;
        }

        expense.Update(description, amount, category, date);
        Save();
        return true;
    }

    public bool Delete(int id)
    {
        Expense? expense = FindById(id);
        if (expense == null)
        {
            return false;
        }

        _expenses.Remove(expense);
        Save();
        return true;
    }

    public List<Expense> GetByMonth(int year, int month)
    {
        return GetAll().Where(e => e.IsInMonth(year, month)).ToList();
    }

    public decimal GetMonthTotal(int year, int month)
    {
        return GetByMonth(year, month).Sum(e => e.Amount);
    }

    public Dictionary<Category, decimal> GetTotalsByCategory(int year, int month)
    {
        var totals = new Dictionary<Category, decimal>();
        foreach (Expense expense in GetByMonth(year, month))
        {
            if (!totals.ContainsKey(expense.Category))
            {
                totals[expense.Category] = 0;
            }
            totals[expense.Category] += expense.Amount;
        }
        return totals;
    }

    public List<DateTime> GetMonthsWithExpenses()
    {
        return _expenses
            .Select(e => new DateTime(e.Date.Year, e.Date.Month, 1))
            .Distinct()
            .OrderBy(m => m)
            .ToList();
    }

    private void Save()
    {
        _storage.Save(_expenses);
    }
}
