namespace FosterFlow.Application.Common.Interfaces;

public interface IIdentityService
{
    Task RegisterShelterAsync(string email, string password, string name, string phone, string street, string postalCode, string city, string country);
}
