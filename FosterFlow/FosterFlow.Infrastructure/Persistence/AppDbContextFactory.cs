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
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Database")
            ?? "Server=localhost;Database=FosterFlow;Trusted_Connection=False;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
