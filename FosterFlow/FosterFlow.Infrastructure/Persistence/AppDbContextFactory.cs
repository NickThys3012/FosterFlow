using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace FosterFlow.Infrastructure.Persistence;

/// <summary>
///     Lets EF Core tooling (migrations, migration bundles) construct the context
///     without booting the API host. The connection string only needs to be valid
///     for design-time SQL generation; the real connection is supplied when the
///     migrations are applied (e.g. via the migration bundle's --connection arg or
///     the ConnectionStrings__Database environment variable).
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Database");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string not found. Set ConnectionStrings__Database (or pass --connection when running the migration bundle).");
        }
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
