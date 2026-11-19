using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Milestone05_WebAPI.Dtos;
using Milestone05_WebAPI.Models;
using Milestone05_WebAPI.Services;

namespace Milestone05_WebAPI.Controllers;

[ApiController]
[Route("api/expenses")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseService _expenses;

    public ExpensesController(ExpenseService expenses)
    {
        _expenses = expenses;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [ProducesResponseType(typeof(List<ExpenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<List<ExpenseResponse>> GetAll([FromQuery] ExpenseQuery query)
    {
        List<Expense> expenses = _expenses.GetAll(UserId, query.Search, query.Year, query.Month);
        return Ok(expenses.Select(ExpenseResponse.FromModel).ToList());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<ExpenseResponse> GetById(int id)
    {
        Expense? expense = _expenses.GetById(UserId, id);
        if (expense == null)
        {
            return NotFoundProblem(id);
        }
        return Ok(ExpenseResponse.FromModel(expense));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<ExpenseResponse> Create(CreateExpenseRequest request)
    {
        Expense expense = _expenses.Add(
            UserId,
            request.Description,
            request.Amount!.Value,
            request.Category!.Value,
            request.Date!.Value.ToDateTime(TimeOnly.MinValue));

        var response = ExpenseResponse.FromModel(expense);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<ExpenseResponse> Update(int id, UpdateExpenseRequest request)
    {
        Expense? expense = _expenses.Update(
            UserId,
            id,
            request.Description,
            request.Amount!.Value,
            request.Category!.Value,
            request.Date!.Value.ToDateTime(TimeOnly.MinValue));

        if (expense == null)
        {
            return NotFoundProblem(id);
        }
        return Ok(ExpenseResponse.FromModel(expense));
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<ExpenseResponse> Patch(int id, PatchExpenseRequest request)
    {
        Expense? current = _expenses.GetById(UserId, id);
        if (current == null)
        {
            return NotFoundProblem(id);
        }

        Expense? expense = _expenses.Update(
            UserId,
            id,
            request.Description ?? current.Description,
            request.Amount ?? current.Amount,
            request.Category ?? current.Category,
            request.Date?.ToDateTime(TimeOnly.MinValue) ?? current.Date);

        if (expense == null)
        {
            return NotFoundProblem(id);
        }
        return Ok(ExpenseResponse.FromModel(expense));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        if (!_expenses.Delete(UserId, id))
        {
            return NotFoundProblem(id);
        }
        return NoContent();
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(MonthlySummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<MonthlySummaryResponse> GetSummary([FromQuery] SummaryQuery query)
    {
        int year = query.Year!.Value;
        int month = query.Month!.Value;

        List<CategoryTotal> totals = _expenses.GetSummary(UserId, year, month);
        decimal total = Math.Round(totals.Sum(t => t.Total), 2);

        return Ok(new MonthlySummaryResponse
        {
            Year = year,
            Month = month,
            Total = total,
            NumberOfExpenses = totals.Sum(t => t.NumberOfExpenses),
            Categories = totals.Select(t => new CategoryTotalResponse
            {
                Category = t.CategoryName,
                NumberOfExpenses = t.NumberOfExpenses,
                Total = Math.Round(t.Total, 2),
                Percent = total == 0 ? 0 : (int)Math.Round(t.Total / total * 100)
            }).ToList()
        });
    }

    private ObjectResult NotFoundProblem(int id)
    {
        return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Not found",
            detail: $"No expense with id {id}.");
    }
}
