using FosterFlow.Domain.Enums;
namespace FosterFlow.Domain.Entities;

//extraction of the user entity from the identity package so that the identity stuff is not in the domain layer
public class User
{

    public User(Guid id, string email, string name, UserRole role)
    {
        Id = id;
        Email = email;
        Name = name;
        Role = role;
    }
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public UserRole Role { get; private set; }
}
