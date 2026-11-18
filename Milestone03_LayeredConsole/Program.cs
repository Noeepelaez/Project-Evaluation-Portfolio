using System.Text;
using Milestone03_LayeredConsole.Presentation;
using Milestone03_LayeredConsole.Repositories;
using Milestone03_LayeredConsole.Services;

namespace Milestone03_LayeredConsole;

internal static class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            IUserRepository userRepository = new JsonUserRepository("users.json");
            IExpenseRepository expenseRepository = new JsonExpenseRepository("expenses.json");

            var authService = new AuthService(userRepository);
            var expenseService = new ExpenseService(expenseRepository, authService);

            var ui = new ConsoleUI(authService, expenseService);
            ui.Run();
        }
        catch (InvalidDataException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
