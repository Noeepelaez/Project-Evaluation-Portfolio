using Milestone03_LayeredConsole.Models;

namespace Milestone03_LayeredConsole.Repositories;

public interface IUserRepository
{
    User? GetById(int id);
    User? GetByUsername(string username);
    void Add(User user);
}
