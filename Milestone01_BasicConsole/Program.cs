using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Milestone01_BasicConsole;

internal static class Program
{
    const string DataFile = "expenses.json";
    const string DateFormat = "dd-MM-yyyy";
    const string MonthFormat = "MM-yyyy";

    static readonly string[] Categories =
    {
        "Food", "Rent", "Transport", "Travel", "Leisure", "Studies", "Sport" , "Other"
    };

    static List<Dictionary<string, string>> expenses = new();
    static int nextId = 1;

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        LoadExpenses();

        bool running = true;

        while (running)
        {
            ShowMenu();
            string option = ReadText("Choose an option: ");

            switch (option)
            {
                case "1": AddExpense(); break;
                case "2": ListExpenses(expenses); break;
                case "3": FindExpenses(); break;
                case "4": DeleteExpense(); break;
                case "5": ShowMonthlySummary(); break;
                case "0": running = false; break;
                default: Console.WriteLine("Invalid option, try again."); break;
            }
        }

        Console.WriteLine("Bye! Your expenses are saved in " + DataFile);
    }

    static void ShowMenu()
    {
        decimal monthTotal = 0;
        foreach (var expense in expenses)
        {
            DateTime date = GetDate(expense);
            if (date.Year == DateTime.Today.Year && date.Month == DateTime.Today.Month)
            {
                monthTotal += GetAmount(expense);
            }
        }

        string month = DateTime.Today.ToString(MonthFormat, CultureInfo.InvariantCulture);

        Console.WriteLine();
        Console.WriteLine("---------- EXPENSE TRACKER ----------");
        Console.WriteLine($"This month ({month}): {monthTotal:0.00} €");
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("1) Add expense");
        Console.WriteLine("2) List all expenses");
        Console.WriteLine("3) Find expenses");
        Console.WriteLine("4) Delete expense");
        Console.WriteLine("5) Monthly summary");
        Console.WriteLine("0) Exit");
        Console.WriteLine("-------------------------------------");
    }

    static void AddExpense()
    {
        Console.WriteLine("\n--- Add expense ---");

        string description = ReadText("Description: ");
        while (description.Length == 0)
        {
            description = ReadText("Description cannot be empty: ");
        }

        decimal amount = ReadAmount("Amount (€): ");
        string category = ReadCategory();
        DateTime date = ReadDate("Date (dd-MM-yyyy, empty = today): ");

        var expense = new Dictionary<string, string>
        {
            ["Id"] = nextId.ToString(),
            ["Date"] = date.ToString(DateFormat, CultureInfo.InvariantCulture),
            ["Description"] = description,
            ["Category"] = category,
            ["Amount"] = amount.ToString("0.00", CultureInfo.InvariantCulture)
        };

        expenses.Add(expense);
        nextId++;
        SaveExpenses();

        Console.WriteLine($"Expense added with id {expense["Id"]}.");
    }

    static void ListExpenses(List<Dictionary<string, string>> list)
    {
        if (list.Count == 0)
        {
            Console.WriteLine("No expenses to show.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"{"Id",-4} {"Date",-11} {"Category",-10} {"Amount",10}  Description");
        Console.WriteLine(new string('-', 60));

        decimal total = 0;
        foreach (var expense in list.OrderBy(e => GetDate(e)))
        {
            decimal amount = GetAmount(expense);
            total += amount;
            Console.WriteLine(
                $"{expense["Id"],-4} {expense["Date"],-11} {expense["Category"],-10} {amount,8:0.00} €  {expense["Description"]}");
        }

        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"{list.Count} expense(s). Total: {total:0.00} €");
    }

    static void FindExpenses()
    {
        string text = ReadText("Search text (description or category): ").ToLower();
        var results = new List<Dictionary<string, string>>();

        foreach (var expense in expenses)
        {
            if (expense["Description"].ToLower().Contains(text) ||
                expense["Category"].ToLower().Contains(text))
            {
                results.Add(expense);
            }
        }

        ListExpenses(results);
    }

    static void DeleteExpense()
    {
        string id = ReadText("Id of the expense to delete: ");

        int index = -1;
        for (int i = 0; i < expenses.Count; i++)
        {
            if (expenses[i]["Id"] == id)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Console.WriteLine($"No expense with id {id}.");
            return;
        }

        var expense = expenses[index];
        string answer = ReadText($"Delete '{expense["Description"]}' ({expense["Amount"]} €)? (y/n): ");
        if (answer.ToLower() == "y")
        {
            expenses.RemoveAt(index);
            SaveExpenses();
            Console.WriteLine("Expense deleted.");
        }
        else
        {
            Console.WriteLine("Cancelled.");
        }
    }
    static void ShowMonthlySummary()
    {
        DateTime selected = ReadMonth("Month (MM-yyyy, empty = current month): ");
        string month = selected.ToString(MonthFormat, CultureInfo.InvariantCulture);

        var monthExpenses = new List<Dictionary<string, string>>();
        foreach (var expense in expenses)
        {
            DateTime date = GetDate(expense);
            if (date.Year == selected.Year && date.Month == selected.Month)
            {
                monthExpenses.Add(expense);
            }
        }

        if (monthExpenses.Count == 0)
        {
            Console.WriteLine($"No expenses in {month}.");
            ShowMonthsWithExpenses();
            return;
        }

        decimal total = 0;
        foreach (var expense in monthExpenses)
        {
            total += GetAmount(expense);
        }

        Console.WriteLine($"\n--- Summary for {month} ---");
        foreach (string category in Categories)
        {
            decimal categoryTotal = 0;
            foreach (var expense in monthExpenses)
            {
                if (expense["Category"] == category)
                {
                    categoryTotal += GetAmount(expense);
                }
            }

            if (categoryTotal > 0)
            {
                int percent = (int)Math.Round(categoryTotal / total * 100);
                Console.WriteLine($"{category,-10} {categoryTotal,8:0.00} € {percent,3}%");
            }
        }

        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"{"TOTAL",-10} {total,8:0.00} €");
        Console.WriteLine($"Number of expenses: {monthExpenses.Count}");
    }
    static string ReadText(string text)
    {
        Console.Write(text);
        return (Console.ReadLine() ?? "").Trim();
    }

    static decimal ReadAmount(string text)
    {
        while (true)
        {
            string input = ReadText(text).Replace(',', '.');
            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount)
                && amount > 0)
            {
                return amount;
            }
            Console.WriteLine("Please enter a positive number");
        }
    }

    static DateTime ReadDate(string text)
    {
        while (true)
        {
            string input = ReadText(text);
            if (input.Length == 0)
            {
                return DateTime.Today;
            }
            if (DateTime.TryParseExact(input, DateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            Console.WriteLine("Invalid date. Use the format dd-MM-yyyy");
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

            Console.WriteLine("Invalid month. Use the format MM-yyyy, e.g. 11-2026");
        }
    }

    static void ShowMonthsWithExpenses()
    {
        var months = new List<DateTime>();
        foreach (var expense in expenses)
        {
            DateTime date = GetDate(expense);
            var month = new DateTime(date.Year, date.Month, 1);
            if (date != DateTime.MinValue && !months.Contains(month))
            {
                months.Add(month);
            }
        }

        if (months.Count == 0)
        {
            return;
        }

        months.Sort();
        var names = new List<string>();
        foreach (var month in months)
        {
            names.Add(month.ToString(MonthFormat, CultureInfo.InvariantCulture));
        }
        Console.WriteLine("Months with expenses: " + string.Join(", ", names));
    }

    static string ReadCategory()
    {
        Console.WriteLine("Categories:");
        for (int i = 0; i < Categories.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {Categories[i]}");
        }

        while (true)
        {
            string input = ReadText("Choose a category number: ");
            if (int.TryParse(input, out int number) && number >= 1 && number <= Categories.Length)
            {
                return Categories[number - 1];
            }
            Console.WriteLine($"Enter a number between 1 and {Categories.Length}.");
        }
    }

    static decimal GetAmount(Dictionary<string, string> expense)
    {
        return decimal.Parse(expense["Amount"], CultureInfo.InvariantCulture);
    }

    static DateTime GetDate(Dictionary<string, string> expense)
    {
        if (DateTime.TryParseExact(expense["Date"], DateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime date))
        {
            return date;
        }
        return DateTime.MinValue;
    }

    static void LoadExpenses()
    {
        if (!File.Exists(DataFile))
        {
            return;
        }

        try
        {
            string json = File.ReadAllText(DataFile);
            expenses = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json) ?? new();
        }
        catch (JsonException)
        {
            Console.WriteLine($"Warning: {DataFile} is corrupted. Starting with an empty list.");
            expenses = new();
        }
        foreach (var expense in expenses)
        {
            if (int.TryParse(expense["Id"], out int id) && id >= nextId)
            {
                nextId = id + 1;
            }
        }

        Console.WriteLine($"Loaded {expenses.Count} expense(s) from {DataFile}.");
    }

    static void SaveExpenses()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(expenses, options);
        File.WriteAllText(DataFile, json);
    }
}
