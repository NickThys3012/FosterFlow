using FosterFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
namespace FosterFlow.Infrastructure.Persistence;

public static class IdentitySeed
{
    public static async Task SeedUsers(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var users = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();
        var roles = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        // Create roles
        foreach (var role in new[]
                 {
                     "Admin", "Shelter", "Foster"
                 })
        {
            if (!await roles.RoleExistsAsync(role))
            {
                await roles.CreateAsync(new IdentityRole(role));
            }
        }

        // Create admin user
        if (await users.FindByEmailAsync("admin@fosterflow.dev") is null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@fosterflow.dev", Email = "admin@fosterflow.dev", DisplayName = "Admin"
            };
            await users.CreateAsync(admin, "Admin1234!");
            await users.AddToRoleAsync(admin, "Admin");
        }
    }
}
