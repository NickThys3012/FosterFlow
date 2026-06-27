using FosterFlow.Domain.Enums;
using Microsoft.AspNetCore.Identity;
namespace FosterFlow.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    // Role stored here for Identity purposes; Domain.User.Role mirrors it
    public UserRole Role { get; set; } // UserRole enum from Domain — safe to reference

    public string? FirstName { get; set; }

    /// <summary>
    ///     Name of the Shelter or the last name of the Foster
    /// </summary>
    public string Name { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
