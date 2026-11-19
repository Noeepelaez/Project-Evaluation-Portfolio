using System.Globalization;
using Milestone04_Database.Models;
using Milestone04_Database.Services;

namespace Milestone04_Database.Presentation;

public class ConsoleUI
{
    private readonly AuthService _auth;
    private readonly ExpenseService _expenses;

    public ConsoleUI(AuthService auth, ExpenseService expenses)
    {
        _auth = auth;
        _expenses = expenses;
    }

    public void Run()
    {
        bool running = true;

        while (running)
        {
            try
            {
                running = _auth.IsLoggedIn ? HandleMainMenu() : HandleAuthMenu();
            }
            catch (ValidationException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        Console.WriteLine("Bye!");
    }

    private bool HandleAuthMenu()
    {
        Console.WriteLine();
        Console.WriteLine("---------- EXPENSE TRACKER ----------");
        Console.WriteLine("1) Register");
        Console.WriteLine("2) Login");
        Console.WriteLine("0) Exit");
        Console.WriteLine("-------------------------------------");

        switch (ConsoleInput.ReadText("Choose an option: "))
        {
            case "1": Register(); break;
            case "2": Login(); break;
            case "0": return false;
            default: Console.WriteLine("Invalid option, try again."); break;
        }
        return true;
    }

    private bool HandleMainMenu()
    {
        DateTime today = DateTime.Today;
        decimal monthTotal = _expenses.GetMonthTotal(today.Year, today.Month);
        string month = today.ToString(ConsoleInput.MonthFormat, CultureInfo.InvariantCulture);

        Console.WriteLine();
        Console.WriteLine("---------- EXPENSE TRACKER ----------");
        Console.WriteLine($"User: {_auth.CurrentUser!.Username}");
        Console.WriteLine($"This month ({month}): {monthTotal:0.00} €");
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("1) Add expense");
        Console.WriteLine("2) List my expenses");
        Console.WriteLine("3) Find expenses");
        Console.WriteLine("4) Edit expense");
        Console.WriteLine("5) Delete expense");
        Console.WriteLine("6) Monthly summary");
        Console.WriteLine("7) Logout");
        Console.WriteLine("0) Exit");
        Console.WriteLine("-------------------------------------");

        switch (ConsoleInput.ReadText("Choose an option: "))
        {
            case "1": AddExpense(); break;
            case "2": PrintExpenses(_expenses.GetAll()); break;
            case "3": FindExpenses(); break;
            case "4": EditExpense(); break;
            case "5": DeleteExpense(); break;
            case "6": ShowMonthlySummary(); break;
            case "7": Logout(); break;
            case "0": return false;
            default: Console.WriteLine("Invalid option, try again."); break;
        }
        return true;
    }

    private void Register()
    {
        Console.WriteLine("\n--- Register ---");
        Console.WriteLine("Leave a field empty to cancel.");

        string? username = AskUntilValid(() =>
        {
            string value = ConsoleInput.ReadText("Username: ");
            if (value.Length == 0)
            {
                return null;
            }
            _auth.ValidateUsername(value);
            return value;
        });
        if (username == null)
        {
            return;
        }

        string? password = AskUntilValid(() =>
        {
            string value = ConsoleInput.ReadPassword("Password: ");
            if (value.Length == 0)
            {
                return null;
            }
            _auth.ValidatePassword(value);
            string repeat = ConsoleInput.ReadPassword("Repeat password: ");
            if (value != repeat)
            {
                throw new ValidationException("The passwords do not match.");
            }
            return value;
        });
        if (password == null)
        {
            return;
        }

        User user = _auth.Register(username, password);
        Console.WriteLine($"User '{user.Username}' created. You can now log in.");
    }

    private void Login()
    {
        Console.WriteLine("\n--- Login ---");
        Console.WriteLine("Leave a field empty to cancel.");

        string? loggedUser = AskUntilValid(() =>
        {
            string username = ConsoleInput.ReadText("Username: ");
            if (username.Length == 0)
            {
                return null;
            }
            string password = ConsoleInput.ReadPassword("Password: ");
            if (password.Length == 0)
            {
                return null;
            }
            return _auth.Login(username, password).Username;
        });

        if (loggedUser != null)
        {
            Console.WriteLine($"Welcome, {loggedUser}!");
        }
    }

    private static string? AskUntilValid(Func<string?> ask)
    {
        while (true)
        {
            try
            {
                return ask();
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"Error: {ex.Message} Try again.");
            }
        }
    }

    private void Logout()
    {
        string username = _auth.CurrentUser!.Username;
        _auth.Logout();
        Console.WriteLine($"Goodbye, {username}. You have logged out.");
    }

    private void AddExpense()
    {
        Console.WriteLine("\n--- Add expense ---");
        string description = ConsoleInput.ReadRequiredText("Description: ");
        decimal amount = ConsoleInput.ReadAmount("Amount (€): ");
        Category category = ConsoleInput.ReadCategory();
        DateTime date = ConsoleInput.ReadDate($"Date ({ConsoleInput.DateFormat}, empty = today): ");

        Expense expense = _expenses.Add(description, amount, category, date);
        Console.WriteLine($"Expense added with id {expense.Id}.");
    }

    private void PrintExpenses(List<Expense> expenses)
    {
        if (expenses.Count == 0)
        {
            Console.WriteLine("No expenses to show.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"{"Id",-4} {"Date",-11} {"Category",-10} {"Amount",10}  Description");
        Console.WriteLine(new string('-', 60));

        foreach (Expense expense in expenses)
        {
            Console.WriteLine(expense);
        }

        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"{expenses.Count} expense(s). Total: {expenses.Sum(e => e.Amount):0.00} €");
    }

    private void FindExpenses()
    {
        string text = ConsoleInput.ReadText("Search text (description or category): ");
        PrintExpenses(_expenses.Search(text));
    }

    private void EditExpense()
    {
        int id = ConsoleInput.ReadInt("Id of the expense to edit: ");
        Expense? expense = _expenses.GetById(id);
        if (expense == null)
        {
            Console.WriteLine($"No expense with id {id}.");
            return;
        }

        Console.WriteLine("Current: " + expense);
        Console.WriteLine("Press Enter to keep the current value.");

        string description = ConsoleInput.ReadText($"Description [{expense.Description}]: ");
        if (description.Length == 0)
        {
            description = expense.Description;
        }

        decimal amount = ConsoleInput.ReadAmount($"Amount [{expense.Amount:0.00}]: ", expense.Amount);
        Category category = ConsoleInput.ReadCategory(expense.Category);
        string currentDate = expense.Date.ToString(ConsoleInput.DateFormat, CultureInfo.InvariantCulture);
        DateTime date = ConsoleInput.ReadDate($"Date [{currentDate}]: ", expense.Date);

        _expenses.Update(id, description, amount, category, date);
        Console.WriteLine("Expense updated.");
    }

    private void DeleteExpense()
    {
        int id = ConsoleInput.ReadInt("Id of the expense to delete: ");
        Expense? expense = _expenses.GetById(id);
        if (expense == null)
        {
            Console.WriteLine($"No expense with id {id}.");
            return;
        }

        string answer = ConsoleInput.ReadText($"Delete '{expense.Description}' ({expense.Amount:0.00} €)? (y/n): ");
        if (answer.ToLower() == "y")
        {
            _expenses.Delete(id);
            Console.WriteLine("Expense deleted.");
        }
        else
        {
            Console.WriteLine("Cancelled.");
        }
    }

    private void ShowMonthlySummary()
    {
        DateTime month = ConsoleInput.ReadMonth($"Month ({ConsoleInput.MonthFormat}, empty = current month): ");
        string monthText = month.ToString(ConsoleInput.MonthFormat, CultureInfo.InvariantCulture);

        List<CategoryTotal> totals = _expenses.GetTotalsByCategory(month.Year, month.Month);
        if (totals.Count == 0)
        {
            Console.WriteLine($"No expenses in {monthText}.");
            ShowMonthsWithExpenses();
            return;
        }

        decimal total = totals.Sum(t => t.Total);

        Console.WriteLine($"\n--- Summary for {monthText} ---");
        foreach (CategoryTotal item in totals)
        {
            int percent = (int)Math.Round(item.Total / total * 100);
            Console.WriteLine($"{item.CategoryName,-10} {item.Total,8:0.00} € {percent,3}%  ({item.NumberOfExpenses})");
        }

        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"{"TOTAL",-10} {total,8:0.00} €");
        Console.WriteLine($"Number of expenses: {totals.Sum(t => t.NumberOfExpenses)}");
    }

    private void ShowMonthsWithExpenses()
    {
        List<DateTime> months = _expenses.GetMonthsWithExpenses();
        if (months.Count > 0)
        {
            var names = months.Select(m => m.ToString(ConsoleInput.MonthFormat, CultureInfo.InvariantCulture));
            Console.WriteLine("Months with expenses: " + string.Join(", ", names));
        }
    }
}
