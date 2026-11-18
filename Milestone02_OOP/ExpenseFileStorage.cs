using System.Text.Json;
using System.Text.Json.Serialization;
using Milestone02_OOP.Models;

namespace Milestone02_OOP;

public class ExpenseFileStorage
{
    private readonly string _filePath;

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public ExpenseFileStorage(string filePath)
    {
        _filePath = filePath;
    }

    public List<Expense> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Expense>();
        }

        try
        {
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Expense>>(json, Options) ?? new List<Expense>();
        }
        catch (Exception)
        {
            Console.WriteLine($"Warning: {_filePath} has invalid data. Starting with an empty list.");
            return new List<Expense>();
        }
    }

    public void Save(IEnumerable<Expense> expenses)
    {
        string json = JsonSerializer.Serialize(expenses, Options);
        File.WriteAllText(_filePath, json);
    }
}
