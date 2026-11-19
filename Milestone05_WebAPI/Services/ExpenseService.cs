using Milestone05_WebAPI.Models;
using Milestone05_WebAPI.Repositories;

namespace Milestone05_WebAPI.Services;

public class ExpenseService
{
    private const int MaxDescriptionLength = 100;
    private const decimal MaxAmount = 10000;
    private static readonly DateTime MinDate = new DateTime(2000, 1, 1);

    private readonly IExpenseRepository _expenses;

    public ExpenseService(IExpenseRepository expenses)
    {
        _expenses = expenses;
    }

    public List<Expense> GetAll(int userId, string? search, int? year, int? month)
    {
        List<Expense> expenses = string.IsNullOrWhiteSpace(search)
            ? _expenses.GetByUser(userId)
            : _expenses.Search(userId, search.Trim());

        if (year != null && month != null)
        {
            expenses = expenses.Where(e => e.Date.Year == year && e.Date.Month == month).ToList();
        }

        return expenses;
    }

    public Expense? GetById(int userId, int id)
    {
        Expense? expense = _expenses.GetById(id);
        if (expense == null || expense.UserId != userId)
        {
            return null;
        }
        return expense;
    }

    public Expense Add(int userId, string description, decimal amount, Category category, DateTime date)
    {
        Validate(description, amount, category, date);

        var expense = new Expense
        {
            UserId = userId,
            Description = description.Trim(),
            Amount = amount,
            Category = category,
            Date = date.Date
        };

        _expenses.Add(expense);
        return expense;
    }

    public Expense? Update(int userId, int id, string description, decimal amount, Category category, DateTime date)
    {
        Expense? expense = GetById(userId, id);
        if (expense == null)
        {
            return null;
        }

        Validate(description, amount, category, date);

        expense.Description = description.Trim();
        expense.Amount = amount;
        expense.Category = category;
        expense.Date = date.Date;

        _expenses.Update(expense);
        return expense;
    }

    public bool Delete(int userId, int id)
    {
        Expense? expense = GetById(userId, id);
        if (expense == null)
        {
            return false;
        }

        _expenses.Delete(expense.Id, userId);
        return true;
    }

    public List<CategoryTotal> GetSummary(int userId, int year, int month)
    {
        return _expenses.GetTotalsByCategory(userId, year, month);
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
            throw new ValidationException($"Amount cannot be greater than {MaxAmount}.");
        }
        if (amount != Math.Round(amount, 2))
        {
            throw new ValidationException("Amount can have at most 2 decimals.");
        }
        if (!Enum.IsDefined(category))
        {
            throw new ValidationException("Invalid category.");
        }
        if (date.Date > DateTime.Today)
        {
            throw new ValidationException("Date cannot be in the future.");
        }
        if (date.Date < MinDate)
        {
            throw new ValidationException("Date cannot be before 01-01-2000.");
        }
    }
}
