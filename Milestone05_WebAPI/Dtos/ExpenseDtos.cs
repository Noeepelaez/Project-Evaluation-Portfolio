using System.ComponentModel.DataAnnotations;
using Milestone05_WebAPI.Models;

namespace Milestone05_WebAPI.Dtos;

public class ExpenseResponse
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public Category Category { get; set; }
    public DateOnly Date { get; set; }

    public static ExpenseResponse FromModel(Expense expense)
    {
        return new ExpenseResponse
        {
            Id = expense.Id,
            Description = expense.Description,
            Amount = Math.Round(expense.Amount, 2),
            Category = expense.Category,
            Date = DateOnly.FromDateTime(expense.Date)
        };
    }
}

public class CreateExpenseRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Description { get; set; } = "";

    [Required]
    [Range(0.01, 10000)]
    public decimal? Amount { get; set; }

    [Required]
    [EnumDataType(typeof(Category))]
    public Category? Category { get; set; }

    [Required]
    public DateOnly? Date { get; set; }
}

public class UpdateExpenseRequest : CreateExpenseRequest
{
}

public class PatchExpenseRequest : IValidatableObject
{
    [StringLength(100, MinimumLength = 1)]
    public string? Description { get; set; }

    [Range(0.01, 10000)]
    public decimal? Amount { get; set; }

    [EnumDataType(typeof(Category))]
    public Category? Category { get; set; }

    public DateOnly? Date { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Description == null && Amount == null && Category == null && Date == null)
        {
            yield return new ValidationResult("Send at least one field to update.");
        }
    }
}

public class ExpenseQuery : IValidatableObject
{
    [StringLength(100)]
    public string? Search { get; set; }

    [Range(2000, 2100)]
    public int? Year { get; set; }

    [Range(1, 12)]
    public int? Month { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if ((Year == null) != (Month == null))
        {
            yield return new ValidationResult("Year and month must be used together.", new[] { nameof(Year), nameof(Month) });
        }
    }
}

public class SummaryQuery
{
    [Required]
    [Range(2000, 2100)]
    public int? Year { get; set; }

    [Required]
    [Range(1, 12)]
    public int? Month { get; set; }
}

public class CategoryTotalResponse
{
    public string Category { get; set; } = "";
    public int NumberOfExpenses { get; set; }
    public decimal Total { get; set; }
    public int Percent { get; set; }
}

public class MonthlySummaryResponse
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Total { get; set; }
    public int NumberOfExpenses { get; set; }
    public List<CategoryTotalResponse> Categories { get; set; } = new();
}
