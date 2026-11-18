using System.Globalization;
using System.Text.Json.Serialization;

namespace Milestone02_OOP.Models;

public class Expense
{
    private string _description = "";
    private decimal _amount;

    public int Id { get; }

    public string Description
    {
        get => _description;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Description cannot be empty.");
            }
            _description = value.Trim();
        }
    }

    public decimal Amount
    {
        get => _amount;
        private set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Amount must be greater than 0.");
            }
            _amount = value;
        }
    }

    public Category Category { get; private set; }

    public DateTime Date { get; private set; }

    [JsonConstructor]
    public Expense(int id, string description, decimal amount, Category category, DateTime date)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Id must be greater than 0.");
        }

        Id = id;
        Description = description;
        Amount = amount;
        Category = category;
        Date = DateTime.SpecifyKind(date.Date, DateTimeKind.Unspecified);
    }

    public void Update(string description, decimal amount, Category category, DateTime date)
    {
        Description = description;
        Amount = amount;
        Category = category;
        Date = DateTime.SpecifyKind(date.Date, DateTimeKind.Unspecified);
    }

    public bool Matches(string text)
    {
        return Description.Contains(text, StringComparison.OrdinalIgnoreCase)
            || Category.ToString().Contains(text, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsInMonth(int year, int month)
    {
        return Date.Year == year && Date.Month == month;
    }

    public override string ToString()
    {
        string date = Date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
        return $"{Id,-4} {date,-11} {Category,-10} {Amount,8:0.00} €  {Description}";
    }
}
