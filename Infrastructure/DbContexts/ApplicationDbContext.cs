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
        var seedId = new UserId(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        var seedTime = new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = seedId,
                FirstName = "Sara",
                LastName = "Rasoulian",
                UserName = "sara",
                Email = new Email("sara@gmail.com"),
                Password = "123456",
                CreatedAt = seedTime,
                CreatedBy = 0,
                LastUpdatedAt = seedTime,
                LastUpdatedBy = 0
            });
    }
}