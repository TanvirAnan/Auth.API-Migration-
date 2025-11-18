using Domain.Entites;
using Domain.Interfaces;
using Domain.ValueObjects;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.Repositories;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDistributedCache _distributedCache;

    public UserRepository(ApplicationDbContext dbContext, IDistributedCache distributedCache)
    {
        _dbContext = dbContext;
        _distributedCache = distributedCache;
    }

    public async Task<User?> GetAsync(UserId userId)
    {
        return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(c => c.Id == userId);
    }

    public async Task<User?> GetAsync(string userName, string password)
    {
        return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(c => c.UserName.ToLower() == userName.ToLower()
                                                                              && c.Password == password);
    }

    public async Task<User?> GetFromCacheAsync(UserId userId)
    {
        var key = $"user-{userId.Value}";
        string? cachedUser = await _distributedCache.GetStringAsync(key);

        if (!string.IsNullOrEmpty(cachedUser))
        {
            return JsonConvert.DeserializeObject<User>(cachedUser);
        }

        var user = await GetAsync(userId);
        if (user is not null)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(user), cacheOptions);
        }

        return user;
    }

    public User Update(User user)
    {
        var result = _dbContext.Users.Update(user);
        return result.Entity;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}