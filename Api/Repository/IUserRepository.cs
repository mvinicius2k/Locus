using Api.Models;
using Shared.Models;

namespace Api;

public interface IUserRepository
{
    public ValueTask<User?> GetById(string id);
    public ValueTask<User?> GetByEmail(string email);
    public ValueTask Add(); 
    public ValueTask Delete(string id);


}

