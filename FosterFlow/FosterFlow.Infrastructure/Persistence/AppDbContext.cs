using FosterFlow.Domain.Entities;
using FosterFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace FosterFlow.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Cat> Cats => Set<Cat>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<RefreshToken>(e =>
        {
            e.HasIndex(t => t.TokenHash).IsUnique();
            e.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId);
        });

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
