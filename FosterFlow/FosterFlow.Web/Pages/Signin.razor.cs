using FosterFlow.Contracts.DTOs.Auth;
using FosterFlow.Web.Services;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages;

public partial class Signin : ComponentBase
{
    private readonly AuthService _auth;
    private readonly NavigationManager _nav;

    private string _email = "";
    private string? _error;
    private string _firstName = "";
    private bool _loading;
    private string _name = "";
    private string _password = "";
    private string _role = "";

    public Signin(AuthService auth, NavigationManager nav)
    {
        _auth = auth;
        _nav = nav;
    }
    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }

    private async Task HandleSignin()
    {
        _loading = true;
        _error = null;

        var ok = await _auth.SigninAsync(new RegisterUserRequest
        {
            Email = _email,
            Password = _password,
            Role = _role,
            Name = _name,
            FirstName = _firstName
        });

        if (!ok)
        {
            _error = "Invalid email or password.";
            _loading = false;
            return;
        }

        _nav.NavigateTo(ReturnUrl ?? "/");
    }
}
