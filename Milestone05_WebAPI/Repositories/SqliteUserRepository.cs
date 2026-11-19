using Dapper;
using Milestone05_WebAPI.Models;

namespace Milestone05_WebAPI.Repositories;

public class SqliteUserRepository : IUserRepository
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public SqliteUserRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public User? GetById(int id)
    {
        using var connection = _connectionFactory.Create();
        return connection.QuerySingleOrDefault<User>(
            "SELECT Id, Username, PasswordHash, PasswordSalt FROM Users WHERE Id = @Id",
            new { Id = id });
    }

    public User? GetByUsername(string username)
    {
        using var connection = _connectionFactory.Create();
        return connection.QuerySingleOrDefault<User>(
            "SELECT Id, Username, PasswordHash, PasswordSalt FROM Users WHERE Username = @Username",
            new { Username = username });
    }

    public void Add(User user)
    {
        const string sql = @"
            INSERT INTO Users (Username, PasswordHash, PasswordSalt)
            VALUES (@Username, @PasswordHash, @PasswordSalt);
            SELECT last_insert_rowid();";

        using var connection = _connectionFactory.Create();
        user.Id = connection.ExecuteScalar<int>(sql, user);
    }
}
