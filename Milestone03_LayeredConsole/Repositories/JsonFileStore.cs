using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milestone03_LayeredConsole.Repositories;

public class JsonFileStore<T>
{
    private readonly string _filePath;

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public JsonFileStore(string filePath)
    {
        _filePath = filePath;
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<T>();
        }

        try
        {
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json, Options) ?? new List<T>();
        }
        catch (JsonException)
        {
            throw new InvalidDataException($"The file {_filePath} is not valid JSON.");
        }
    }

    public void Save(List<T> items)
    {
        string json = JsonSerializer.Serialize(items, Options);
        File.WriteAllText(_filePath, json);
    }
}
