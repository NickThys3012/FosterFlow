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
    // A syntactically valid SQL Server connection string that points nowhere.
    // Enough for offline tooling (migrations add, scaffolding, IDE context creation),
    // which generate SQL without opening a connection. Operations that hit a real
    // database (database update) must supply a connection via ConnectionStrings__Database
    // or the migration bundle's --connection argument.
    private const string DesignTimePlaceholder =
        "Server=(localdb)\\MSSQLLocalDB;Database=FosterFlow_DesignTime;Trusted_Connection=True;TrustServerCertificate=True";

    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Database");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine(
                "ConnectionStrings__Database not set; using a design-time placeholder connection string. " +
                "Set ConnectionStrings__Database (or pass --connection) to target a real database.");
            connectionString = DesignTimePlaceholder;
        }
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
