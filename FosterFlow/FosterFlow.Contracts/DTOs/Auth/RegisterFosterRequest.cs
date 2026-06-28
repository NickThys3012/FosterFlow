using FosterFlow.Domain.Enums;
namespace FosterFlow.Contracts.DTOs.Auth;

public class RegisterFosterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool HasDogs { get; set; }
    public bool HasKids { get; set; }
    public HomeType HomeType { get; set; }
    public ExperienceLevel ExperienceLevel { get; set; }
    public int MaxCats { get; set; } = 1;
    public DateOnly AvailableFrom { get; set; }
    public DateOnly AvailableTo { get; set; }
}
