using Milestone03_LayeredConsole.Models;
using Milestone03_LayeredConsole.Repositories;

namespace Milestone03_LayeredConsole.Services;

public class ExpenseService
{
    private const int MaxDescriptionLength = 100;
    private const decimal MaxAmount = 10000;

    private readonly IExpenseRepository _expenses;
    private readonly AuthService _auth;

    public ExpenseService(IExpenseRepository expenses, AuthService auth)
    {
        _expenses = expenses;
        _auth = auth;
    }

    private int CurrentUserId =>
        _auth.CurrentUser?.Id ?? throw new InvalidOperationException("No user is logged in.");

    public List<Expense> GetAll()
    {
        return _expenses.GetByUser(CurrentUserId)
            .OrderBy(e => e.Date)
            .ThenBy(e => e.Id)
            .ToList();
    }

    public Expense? GetById(int id)
    {
        Expense? expense = _expenses.GetById(id);
        if (expense == null || expense.UserId != CurrentUserId)
        {
            return null;
        }
        return expense;
    }

    public Expense Add(string description, decimal amount, Category category, DateTime date)
    {
        Validate(description, amount, category, date);

        var expense = new Expense
        {
            UserId = CurrentUserId,
            Description = description.Trim(),
            Amount = amount,
            Category = category,
            Date = DateTime.SpecifyKind(date.Date, DateTimeKind.Unspecified)
        };

        _expenses.Add(expense);
        return expense;
    }

    public void Update(int id, string description, decimal amount, Category category, DateTime date)
    {
        Expense expense = GetById(id) ?? throw new ValidationException($"No expense with id {id}.");

        Validate(description, amount, category, date);

        expense.Description = description.Trim();
        expense.Amount = amount;
        expense.Category = category;
        expense.Date = DateTime.SpecifyKind(date.Date, DateTimeKind.Unspecified);

        _expenses.Update(expense);
    }

    public void Delete(int id)
    {
        Expense expense = GetById(id) ?? throw new ValidationException($"No expense with id {id}.");
        _expenses.Delete(expense.Id);
    }

    public List<Expense> Search(string text)
    {
        return GetAll()
            .Where(e => e.Description.Contains(text, StringComparison.OrdinalIgnoreCase)
                     || e.Category.ToString().Contains(text, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Expense> GetByMonth(int year, int month)
    {
        return GetAll().Where(e => e.Date.Year == year && e.Date.Month == month).ToList();
    }

    public decimal GetMonthTotal(int year, int month)
    {
        return GetByMonth(year, month).Sum(e => e.Amount);
    }

    public Dictionary<Category, decimal> GetTotalsByCategory(int year, int month)
    {
        return GetByMonth(year, month)
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
    }

    public List<DateTime> GetMonthsWithExpenses()
    {
        return GetAll()
            .Select(e => new DateTime(e.Date.Year, e.Date.Month, 1))
            .Distinct()
            .OrderBy(m => m)
            .ToList();
    }

    private static void Validate(string description, decimal amount, Category category, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ValidationException("Description cannot be empty.");
        }
        if (description.Trim().Length > MaxDescriptionLength)
        {
            throw new ValidationException($"Description cannot be longer than {MaxDescriptionLength} characters.");
        }
        if (amount <= 0)
        {
            throw new ValidationException("Amount must be greater than 0.");
        }
        if (amount > MaxAmount)
        {
            throw new ValidationException($"Amount cannot be greater than {MaxAmount} €.");
        }
        if (!Enum.IsDefined(category))
        {
            throw new ValidationException("Invalid category.");
        }
        if (date.Date > DateTime.Today)
        {
            throw new ValidationException("Date cannot be in the future.");
        }
    }
}
