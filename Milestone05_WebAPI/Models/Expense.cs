using System.Globalization;

namespace Milestone05_WebAPI.Models;

public class Expense
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public Category Category { get; set; }
    public DateTime Date { get; set; }

    public override string ToString()
    {
        string date = Date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
        return $"{Id,-4} {date,-11} {Category,-10} {Amount,8:0.00} €  {Description}";
    }
}
