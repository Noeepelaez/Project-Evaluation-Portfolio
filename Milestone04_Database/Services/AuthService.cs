using System.Text.RegularExpressions;
using Milestone04_Database.Models;
using Milestone04_Database.Repositories;

namespace Milestone04_Database.Services;

public class AuthService
{
    private readonly IUserRepository _users;

    public AuthService(IUserRepository users)
    {
        _users = users;
    }

    public User? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser != null;

    public void ValidateUsername(string username)
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
        if (_users.GetByUsername(username) != null)
        {
            throw new ValidationException($"The username '{username}' is already taken.");
        }
    }

    public void ValidatePassword(string password)
    {
        if (password.Length < 6 || password.Length > 100)
        {
            throw new ValidationException("Password must have between 6 and 100 characters.");
        }
        if (!password.Any(char.IsDigit))
        {
            throw new ValidationException("Password must contain at least one number.");
        }
    }

    public User Register(string username, string password)
    {
        username = username.Trim();
        ValidateUsername(username);
        ValidatePassword(password);

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

    public User Login(string username, string password)
    {
        User? user = _users.GetByUsername(username.Trim());

        if (user == null || !PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt))
        {
            throw new ValidationException("Invalid username or password.");
        }

        CurrentUser = user;
        return user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}
