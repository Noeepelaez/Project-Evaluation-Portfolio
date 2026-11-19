using System.Globalization;
using Dapper;
using Milestone05_WebAPI.Models;

namespace Milestone05_WebAPI.Repositories;

public class SqliteExpenseRepository : IExpenseRepository
{
    private const string SelectExpense = @"
        SELECT Id, UserId, Description, Amount, CategoryId AS Category, Date
        FROM Expenses";

    private readonly SqliteConnectionFactory _connectionFactory;

    public SqliteExpenseRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public List<Expense> GetByUser(int userId)
    {
        using var connection = _connectionFactory.Create();
        return connection.Query<Expense>(
            SelectExpense + " WHERE UserId = @UserId ORDER BY Date, Id",
            new { UserId = userId }).ToList();
    }

    public Expense? GetById(int id)
    {
        using var connection = _connectionFactory.Create();
        return connection.QuerySingleOrDefault<Expense>(
            SelectExpense + " WHERE Id = @Id",
            new { Id = id });
    }

    public List<Expense> Search(int userId, string text)
    {
        const string sql = @"
            SELECT e.Id, e.UserId, e.Description, e.Amount, e.CategoryId AS Category, e.Date
            FROM Expenses e
            JOIN Categories c ON c.Id = e.CategoryId
            WHERE e.UserId = @UserId
              AND (e.Description LIKE @Pattern ESCAPE '\' OR c.Name LIKE @Pattern ESCAPE '\')
            ORDER BY e.Date, e.Id";

        using var connection = _connectionFactory.Create();
        return connection.Query<Expense>(sql, new { UserId = userId, Pattern = "%" + EscapeLike(text) + "%" }).ToList();
    }

    public List<Expense> GetByMonth(int userId, int year, int month)
    {
        using var connection = _connectionFactory.Create();
        return connection.Query<Expense>(
            SelectExpense + " WHERE UserId = @UserId AND strftime('%Y-%m', Date) = @Month ORDER BY Date, Id",
            new { UserId = userId, Month = ToMonthKey(year, month) }).ToList();
    }

    public decimal GetMonthTotal(int userId, int year, int month)
    {
        const string sql = @"
            SELECT IFNULL(SUM(Amount), 0)
            FROM Expenses
            WHERE UserId = @UserId AND strftime('%Y-%m', Date) = @Month";

        using var connection = _connectionFactory.Create();
        double total = connection.ExecuteScalar<double>(sql, new { UserId = userId, Month = ToMonthKey(year, month) });
        return Math.Round((decimal)total, 2);
    }

    public List<CategoryTotal> GetTotalsByCategory(int userId, int year, int month)
    {
        const string sql = @"
            SELECT c.Name AS CategoryName, COUNT(*) AS NumberOfExpenses, SUM(e.Amount) AS Total
            FROM Expenses e
            JOIN Categories c ON c.Id = e.CategoryId
            WHERE e.UserId = @UserId AND strftime('%Y-%m', e.Date) = @Month
            GROUP BY c.Name
            ORDER BY Total DESC";

        using var connection = _connectionFactory.Create();
        return connection.Query<CategoryTotal>(sql, new { UserId = userId, Month = ToMonthKey(year, month) }).ToList();
    }

    public List<DateTime> GetMonthsWithExpenses(int userId)
    {
        const string sql = @"
            SELECT DISTINCT strftime('%Y-%m', Date) AS Month
            FROM Expenses
            WHERE UserId = @UserId
            ORDER BY Month";

        using var connection = _connectionFactory.Create();
        return connection.Query<string>(sql, new { UserId = userId })
            .Select(m => DateTime.ParseExact(m, "yyyy-MM", CultureInfo.InvariantCulture))
            .ToList();
    }

    public void Add(Expense expense)
    {
        const string sql = @"
            INSERT INTO Expenses (UserId, CategoryId, Description, Amount, Date)
            VALUES (@UserId, @CategoryId, @Description, @Amount, @Date);
            SELECT last_insert_rowid();";

        using var connection = _connectionFactory.Create();
        expense.Id = connection.ExecuteScalar<int>(sql, ToParameters(expense));
    }

    public void Update(Expense expense)
    {
        const string sql = @"
            UPDATE Expenses
            SET CategoryId = @CategoryId, Description = @Description, Amount = @Amount, Date = @Date
            WHERE Id = @Id AND UserId = @UserId";

        using var connection = _connectionFactory.Create();
        connection.Execute(sql, ToParameters(expense));
    }

    public void Delete(int id, int userId)
    {
        using var connection = _connectionFactory.Create();
        connection.Execute("DELETE FROM Expenses WHERE Id = @Id AND UserId = @UserId", new { Id = id, UserId = userId });
    }

    private static object ToParameters(Expense expense)
    {
        return new
        {
            expense.Id,
            expense.UserId,
            CategoryId = (int)expense.Category,
            expense.Description,
            Amount = (double)expense.Amount,
            Date = expense.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        };
    }

    private static string EscapeLike(string text)
    {
        return text.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
    }

    private static string ToMonthKey(int year, int month)
    {
        return $"{year:D4}-{month:D2}";
    }
}
