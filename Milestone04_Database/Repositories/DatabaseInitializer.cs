using Dapper;

namespace Milestone04_Database.Repositories;

public class DatabaseInitializer
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public DatabaseInitializer(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Initialize(string schemaFile)
    {
        string sql = File.ReadAllText(schemaFile);

        using var connection = _connectionFactory.Create();
        connection.Execute(sql);
    }
}
