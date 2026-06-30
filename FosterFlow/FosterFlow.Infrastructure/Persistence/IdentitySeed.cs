using FosterFlow.Domain.Enums;
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
                UserName = "admin@fosterflow.dev",
                Email = "admin@fosterflow.dev",
                Name = "Admin",
                Role = UserRole.Admin,
                PhoneNumber = "0000000000",
                Street = "Street",
                PostalCode = "12345",
                City = "City",
                Country = "Country"
            };
            await users.CreateAsync(admin, "Admin1234!");
            await users.AddToRoleAsync(admin, "Admin");
        }
        if (await users.FindByEmailAsync("shelter@fosterflow.dev") is null)
        {
            var shelter = new ApplicationUser
            {
                UserName = "shelter@fosterflow.dev",
                Email = "shelter@fosterflow.dev",
                Name = "Shelter",
                Role = UserRole.Shelter,
                PhoneNumber = "0000000000",
                Street = "Street",
                PostalCode = "12345",
                City = "City",
                Country = "Country"
            };
            await users.CreateAsync(shelter, "Shelter1234!");
            await users.AddToRoleAsync(shelter, "Shelter");
        }
        if (await users.FindByEmailAsync("foster@fosterflow.dev") is null)
        {
            var foster = new ApplicationUser
            {
                UserName = "foster@fosterflow.dev",
                Email = "foster@fosterflow.dev",
                Name = "Foster",
                Role = UserRole.Foster,
                PhoneNumber = "0000000000",
                Street = "Street",
                PostalCode = "12345",
                City = "City",
                Country = "Country"
            };
            await users.CreateAsync(foster, "Foster1234!");
            await users.AddToRoleAsync(foster, "Foster");
        }
    }
}
