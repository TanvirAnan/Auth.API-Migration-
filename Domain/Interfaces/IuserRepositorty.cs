using Domain.Entites;
using Domain.ValueObjects;

namespace Domain.Interfaces;



public interface IUserRepository
{
    Task<User?> GetAsync(UserId userId);
    Task<User?> GetAsync(string userName, string password);
    Task<User?> GetFromCacheAsync(UserId userId);
    User Update(User user);
    Task SaveChangesAsync();
}
