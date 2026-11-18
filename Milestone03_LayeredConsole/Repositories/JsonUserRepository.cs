using Milestone03_LayeredConsole.Models;

namespace Milestone03_LayeredConsole.Repositories;

public class JsonUserRepository : IUserRepository
{
    private readonly JsonFileStore<User> _store;
    private readonly List<User> _users;

    public JsonUserRepository(string filePath)
    {
        _store = new JsonFileStore<User>(filePath);
        _users = _store.Load();
    }

    public User? GetById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public User? GetByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public void Add(User user)
    {
        user.Id = _users.Count == 0 ? 1 : _users.Max(u => u.Id) + 1;
        _users.Add(user);
        _store.Save(_users);
    }
}
