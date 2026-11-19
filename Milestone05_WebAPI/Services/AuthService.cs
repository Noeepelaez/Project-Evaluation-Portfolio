using System.Text.RegularExpressions;
using Milestone05_WebAPI.Models;
using Milestone05_WebAPI.Repositories;

namespace Milestone05_WebAPI.Services;

public class AuthService
{
    private readonly IUserRepository _users;

    public AuthService(IUserRepository users)
    {
        _users = users;
    }

    public User Register(string username, string password)
    {
        username = username.Trim();

        if (username.Length < 3 || username.Length > 20)
        {
            throw new ValidationException("Username must have between 3 and 20 characters.");
        }
        if (!Regex.IsMatch(username, "^[a-zA-Z0-9_]+$"))
        {
            throw new ValidationException("Username can only contain letters, numbers and _.");
        }
        if (password.Length < 6 || password.Length > 100)
        {
            throw new ValidationException("Password must have between 6 and 100 characters.");
        }
        if (!password.Any(char.IsDigit))
        {
            throw new ValidationException("Password must contain at least one number.");
        }
        if (_users.GetByUsername(username) != null)
        {
            throw new ConflictException($"The username '{username}' is already taken.");
        }

        var (hash, salt) = PasswordHasher.Hash(password);
        var user = new User
        {
            Username = username,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        _users.Add(user);
        return user;
    }

    public User? Authenticate(string username, string password)
    {
        if (password.Length > 100)
        {
            return null;
        }

        User? user = _users.GetByUsername(username.Trim());
        if (user == null || !PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt))
        {
            return null;
        }
        return user;
    }

    public User? GetById(int id)
    {
        return _users.GetById(id);
    }
}
