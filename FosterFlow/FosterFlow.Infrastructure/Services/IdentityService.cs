using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Domain.Enums;
using FosterFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
namespace FosterFlow.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task RegisterAsync(string email, string password, string displayName, UserRole role)
    {
        var appUser = new ApplicationUser
        {
            UserName = email, Email = email, DisplayName = displayName, Role = role
        };

        var result = await _userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
        {
            throw new ProcessingException(nameof(ApplicationUser), appUser.Id);
        }

        await _userManager.AddToRoleAsync(appUser, role.ToString());
    }
}
