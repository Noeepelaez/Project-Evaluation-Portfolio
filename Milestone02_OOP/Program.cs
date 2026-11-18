using System.Globalization;
using System.Text;
using Milestone02_OOP.Models;

namespace Milestone02_OOP;

internal static class Program
{
    const string DataFile = "expenses.json";
    const string DateFormat = "dd-MM-yyyy";
    const string MonthFormat = "MM-yyyy";

    static ExpenseManager manager = null!;

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var storage = new ExpenseFileStorage(DataFile);
        manager = new ExpenseManager(storage);
        Console.WriteLine($"Loaded {manager.Count} expense(s) from {DataFile}.");

        bool running = true;
        while (running)
        {
            ShowMenu();
            string option = ReadText("Choose an option: ");

            switch (option)
            {
                case "1": AddExpense(); break;
                case "2": PrintExpenses(manager.GetAll()); break;
                case "3": FindExpenses(); break;
                case "4": EditExpense(); break;
                case "5": DeleteExpense(); break;
                case "6": ShowMonthlySummary(); break;
                case "0": running = false; break;
                default: Console.WriteLine("Invalid option, try again."); break;
            }
        }

        Console.WriteLine("Bye! Your expenses are saved in " + DataFile);
    }

    static void ShowMenu()
    {
        DateTime today = DateTime.Today;
        decimal monthTotal = manager.GetMonthTotal(today.Year, today.Month);
        string month = today.ToString(MonthFormat, CultureInfo.InvariantCulture);

        Console.WriteLine();
        Console.WriteLine("---------- EXPENSE TRACKER ----------");
        Console.WriteLine($"This month ({month}): {monthTotal:0.00} €");
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("1) Add expense");
        Console.WriteLine("2) List all expenses");
        Console.WriteLine("3) Find expenses");
        Console.WriteLine("4) Edit expense");
        Console.WriteLine("5) Delete expense");
        Console.WriteLine("6) Monthly summary");
        Console.WriteLine("0) Exit");
        Console.WriteLine("-------------------------------------");
    }

    static void AddExpense()
    {
        Console.WriteLine("\n--- Add expense ---");
        string description = ReadRequiredText("Description: ");
        decimal amount = ReadAmount("Amount (€): ");
        Category category = ReadCategory();
        DateTime date = ReadDate($"Date ({DateFormat}, empty = today): ");

        try
        {
            Expense expense = manager.Add(description, amount, category, date);
            Console.WriteLine($"Expense added with id {expense.Id}.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static void PrintExpenses(List<Expense> expenses)
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

    static void FindExpenses()
    {
        string text = ReadText("Search text (description or category): ");
        PrintExpenses(manager.Search(text));
    }

    static void EditExpense()
    {
        int id = ReadInt("Id of the expense to edit: ");
        Expense? expense = manager.FindById(id);
        if (expense == null)
        {
            Console.WriteLine($"No expense with id {id}.");
            return;
        }

        Console.WriteLine("Current: " + expense);
        Console.WriteLine("Press Enter to keep the current value.");

        string description = ReadText($"Description [{expense.Description}]: ");
        if (description.Length == 0)
        {
            description = expense.Description;
        }

        decimal amount = ReadAmount($"Amount [{expense.Amount:0.00}]: ", expense.Amount);
        Category category = ReadCategory(expense.Category);
        DateTime date = ReadDate(
            $"Date [{expense.Date.ToString(DateFormat, CultureInfo.InvariantCulture)}]: ", expense.Date);

        try
        {
            manager.Update(id, description, amount, category, date);
            Console.WriteLine("Expense updated.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static void DeleteExpense()
    {
        int id = ReadInt("Id of the expense to delete: ");
        Expense? expense = manager.FindById(id);
        if (expense == null)
        {
            Console.WriteLine($"No expense with id {id}.");
            return;
        }

        string answer = ReadText($"Delete '{expense.Description}' ({expense.Amount:0.00} €)? (y/n): ");
        if (answer.ToLower() == "y")
        {
            manager.Delete(id);
            Console.WriteLine("Expense deleted.");
        }
        else
        {
            Console.WriteLine("Cancelled.");
        }
    }

    static void ShowMonthlySummary()
    {
        DateTime month = ReadMonth($"Month ({MonthFormat}, empty = current month): ");
        string monthText = month.ToString(MonthFormat, CultureInfo.InvariantCulture);

        Dictionary<Category, decimal> totals = manager.GetTotalsByCategory(month.Year, month.Month);
        if (totals.Count == 0)
        {
            Console.WriteLine($"No expenses in {monthText}.");
            ShowMonthsWithExpenses();
            return;
        }

        decimal total = totals.Values.Sum();

        Console.WriteLine($"\n--- Summary for {monthText} ---");
        foreach (var pair in totals.OrderByDescending(p => p.Value))
        {
            int percent = (int)Math.Round(pair.Value / total * 100);
            Console.WriteLine($"{pair.Key,-10} {pair.Value,8:0.00} € {percent,3}%");
        }

        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"{"TOTAL",-10} {total,8:0.00} €");
        Console.WriteLine($"Number of expenses: {manager.GetByMonth(month.Year, month.Month).Count}");
    }

    static void ShowMonthsWithExpenses()
    {
        List<DateTime> months = manager.GetMonthsWithExpenses();
        if (months.Count > 0)
        {
            var names = months.Select(m => m.ToString(MonthFormat, CultureInfo.InvariantCulture));
            Console.WriteLine("Months with expenses: " + string.Join(", ", names));
        }
    }

    static string ReadText(string text)
    {
        Console.Write(text);
        return (Console.ReadLine() ?? "").Trim();
    }

    static string ReadRequiredText(string text)
    {
        while (true)
        {
            string input = ReadText(text);
            if (input.Length > 0)
            {
                return input;
            }
            Console.WriteLine("Error: this field cannot be empty.");
        }
    }

    static int ReadInt(string text)
    {
        while (true)
        {
            if (int.TryParse(ReadText(text), out int number))
            {
                return number;
            }
            Console.WriteLine("Please enter a whole number.");
        }
    }

    static decimal ReadAmount(string text, decimal? current = null)
    {
        while (true)
        {
            string input = ReadText(text).Replace(',', '.');
            if (input.Length == 0 && current != null)
            {
                return current.Value;
            }
            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount)
                && amount > 0)
            {
                return amount;
            }
            Console.WriteLine("Please enter a positive number");
        }
    }

    static DateTime ReadDate(string text, DateTime? current = null)
    {
        while (true)
        {
            string input = ReadText(text);
            if (input.Length == 0)
            {
                return current ?? DateTime.Today;
            }
            if (DateTime.TryParseExact(input, DateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            Console.WriteLine($"Invalid date. Use the format {DateFormat}");
        }
    }

    static DateTime ReadMonth(string text)
    {
        while (true)
        {
            string input = ReadText(text);
            if (input.Length == 0)
            {
                return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }
            if (DateTime.TryParseExact(input, MonthFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            Console.WriteLine($"Invalid month. Use the format {MonthFormat}, e.g. 11-2026");
        }
    }

    static Category ReadCategory(Category? current = null)
    {
        Category[] categories = Enum.GetValues<Category>();

        Console.WriteLine("Categories:");
        for (int i = 0; i < categories.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {categories[i]}");
        }

        string prompt = current == null
            ? "Choose a category number: "
            : $"Choose a category number [{current}]: ";

        while (true)
        {
            string input = ReadText(prompt);
            if (input.Length == 0 && current != null)
            {
                return current.Value;
            }
            if (int.TryParse(input, out int number) && number >= 1 && number <= categories.Length)
            {
                return categories[number - 1];
            }
            Console.WriteLine($"Enter a number between 1 and {categories.Length}.");
        }
    }
}
