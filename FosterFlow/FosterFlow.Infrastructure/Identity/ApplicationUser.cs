using FosterFlow.Domain.Enums;
using Microsoft.AspNetCore.Identity;
namespace FosterFlow.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    // Display name stored here, synced to Domain.User.Name on read
    public string DisplayName { get; set; }

    // Role stored here for Identity purposes; Domain.User.Role mirrors it
    public UserRole Role { get; set; } // UserRole enum from Domain — safe to reference
}
