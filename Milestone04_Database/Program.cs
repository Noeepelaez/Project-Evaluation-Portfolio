using System.Text;
using Dapper;
using Microsoft.Data.Sqlite;
using Milestone04_Database.Presentation;
using Milestone04_Database.Repositories;
using Milestone04_Database.Services;

namespace Milestone04_Database;

internal static class Program
{
    const string DatabaseFile = "expenses.db";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            SqlMapper.AddTypeHandler(new DateTypeHandler());

            var connectionFactory = new SqliteConnectionFactory(DatabaseFile);
            string schemaFile = Path.Combine(AppContext.BaseDirectory, "Database", "schema.sql");
            new DatabaseInitializer(connectionFactory).Initialize(schemaFile);

            IUserRepository userRepository = new SqliteUserRepository(connectionFactory);
            IExpenseRepository expenseRepository = new SqliteExpenseRepository(connectionFactory);

            var authService = new AuthService(userRepository);
            var expenseService = new ExpenseService(expenseRepository, authService);

            var ui = new ConsoleUI(authService, expenseService);
            ui.Run();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
    }
}
