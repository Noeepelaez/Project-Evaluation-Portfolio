using Microsoft.Data.Sqlite;

namespace Milestone04_Database.Repositories;

public class SqliteConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string databaseFile)
    {
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databaseFile,
            ForeignKeys = true
        }.ToString();
    }

    public SqliteConnection Create()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
