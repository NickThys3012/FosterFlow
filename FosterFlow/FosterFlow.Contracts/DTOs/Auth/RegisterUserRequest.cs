namespace FosterFlow.Contracts.DTOs.Auth;

public class RegisterUserRequest
{

    public string Email { get; set; }
    public string Password { get; set; }
    public string Name
    {
        get;
        set;
    }
    public string FirstName { get; set; }
    public string Role { get; set; }
}
