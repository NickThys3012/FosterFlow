using FosterFlow.Domain.Entities;
using FosterFlow.Domain.Entities.Base;
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

        builder.Entity<Cat>(e =>
        {
            e.HasOne<ApplicationUser>()
                .WithMany(u => u.Cats)
                .HasForeignKey(c => c.ShelterId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Restrict); // keep cats if shelter user is deleted
        });
        builder.Entity<Cat>().HasQueryFilter(c => !c.IsDeleted);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
    {
        foreach (var entity in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entity.State)
            {
                case EntityState.Added:
                    entity.Entity.CreatedDate = DateTime.UtcNow;
                    entity.Entity.UpdatedDate = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entity.Entity.UpdatedDate = DateTime.UtcNow;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    break;
            }
        }
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
