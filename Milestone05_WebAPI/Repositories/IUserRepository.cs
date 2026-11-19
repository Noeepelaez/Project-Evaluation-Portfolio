using Milestone05_WebAPI.Models;

namespace Milestone05_WebAPI.Repositories;

public interface IUserRepository
{
    User? GetById(int id);
    User? GetByUsername(string username);
    void Add(User user);
}
