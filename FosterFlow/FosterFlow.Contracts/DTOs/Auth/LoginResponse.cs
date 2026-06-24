namespace FosterFlow.Contracts.DTOs.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; }
    public DateTime Expiration { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
