using Domain.Entites;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.DbContexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Seed sample data
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = new UserId(Guid.NewGuid()),
                FirstName = "Sara",
                LastName = "Rasoulian",
                UserName = "sara",
                Email = new Email("sara@gmail.com"),
                Password = "123456",
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = 0,
                LastUpdatedAt = DateTimeOffset.UtcNow,
                LastUpdatedBy = 0
            });
    }
}