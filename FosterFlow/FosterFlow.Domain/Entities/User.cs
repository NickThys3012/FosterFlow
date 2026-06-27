using FosterFlow.Domain.Enums;
namespace FosterFlow.Domain.Entities;

//extraction of the user entity from the identity package so that the identity stuff is not in the domain layer
public class User
{

    public User(Guid id, string email, string name, UserRole role, string street, string postalCode, string city, string country, string? firstName)
    {
        Id = id;
        Email = email;
        Name = name;
        Role = role;
        Street = street;
        PostalCode = postalCode;
        City = city;
        Country = country;
        FirstName = firstName;
    }
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public UserRole Role { get; private set; }
    public string? FirstName { get; private set; }

    public string Street { get; private set; }
    public string PostalCode { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }
}
