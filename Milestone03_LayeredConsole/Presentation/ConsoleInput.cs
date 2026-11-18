using System.Globalization;
using System.Text;
using Milestone03_LayeredConsole.Models;

namespace Milestone03_LayeredConsole.Presentation;

public static class ConsoleInput
{
    public const string DateFormat = "dd-MM-yyyy";
    public const string MonthFormat = "MM-yyyy";

    public static string ReadText(string text)
    {
        Console.Write(text);
        return (Console.ReadLine() ?? "").Trim();
    }

    public static string ReadRequiredText(string text)
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

    public static string ReadPassword(string text)
    {
        Console.Write(text);
        var password = new StringBuilder();

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return password.ToString();
            }
            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Length--;
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write('*');
            }
        }
    }

    public static int ReadInt(string text)
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

    public static decimal ReadAmount(string text, decimal? current = null)
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
            Console.WriteLine("Please enter a positive number.");
        }
    }

    public static DateTime ReadDate(string text, DateTime? current = null)
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
            Console.WriteLine($"Invalid date. Use the format {DateFormat}.");
        }
    }

    public static DateTime ReadMonth(string text)
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
            Console.WriteLine($"Invalid month. Use the format {MonthFormat}, e.g. 11-2026.");
        }
    }

    public static Category ReadCategory(Category? current = null)
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
