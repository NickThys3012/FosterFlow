using FosterFlow.Domain.Enums;
namespace FosterFlow.Application.Common.Interfaces;

public interface IIdentityService
{
    Task RegisterShelterAsync(string email, string password, string name, string phone, string street, string postalCode, string city, string country);
    Task RegisterFosterAsync(string email, string password, string name, string phone, string street, string postalCode, string city, string country, ExperienceLevel experienceLevel, HomeType homeType, bool hasKids, bool hasDogs, int maxCats, DateOnly availableFrom, DateOnly availableTo);
}
