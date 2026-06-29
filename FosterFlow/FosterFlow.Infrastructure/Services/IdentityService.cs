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

    public async Task RegisterShelterAsync(string email, string password, string name, string phone, string street, string postalCode, string city, string country)
    {
        var appUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            Name = name,
            Role = UserRole.Shelter,
            PhoneNumber = phone,
            Street = street,
            PostalCode = postalCode,
            City = city,
            Country = country
        };

        var result = await _userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
        {
            throw new ProcessingException(nameof(ApplicationUser), appUser.Id);
        }

        await _userManager.AddToRoleAsync(appUser, appUser.Role.ToString());
    }
    public async Task RegisterFosterAsync(
        string email,
        string password,
        string firstName,
        string name,
        string phone,
        string street,
        string postalCode,
        string city,
        string country,
        ExperienceLevel experienceLevel,
        HomeType homeType,
        bool hasKids,
        bool hasDogs,
        int maxCats,
        DateOnly availableFrom,
        DateOnly availableTo)
    {
        var appUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            Name = name,
            Role = UserRole.Foster,
            PhoneNumber = phone,
            Street = street,
            PostalCode = postalCode,
            City = city,
            Country = country,
            ExperienceLevel = experienceLevel,
            HomeType = homeType,
            HasKids = hasKids,
            HasDogs = hasDogs,
            MaxCats = maxCats,
            AvailableFrom = availableFrom,
            AvailableTo = availableTo
        };

        var result = await _userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
        {
            throw new ProcessingException(nameof(ApplicationUser), appUser.Id);
        }

        await _userManager.AddToRoleAsync(appUser, appUser.Role.ToString());
    }
}
