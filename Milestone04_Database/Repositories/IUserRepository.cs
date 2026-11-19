using Milestone04_Database.Models;

namespace Milestone04_Database.Repositories;

public interface IUserRepository
{
    User? GetById(int id);
    User? GetByUsername(string username);
    void Add(User user);
}
