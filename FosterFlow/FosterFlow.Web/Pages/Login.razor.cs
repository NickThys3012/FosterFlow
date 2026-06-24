using FosterFlow.Contracts.DTOs.Auth;
using FosterFlow.Web.Services;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages;

public partial class Login : ComponentBase
{
    private readonly AuthService _auth;
    private readonly NavigationManager _nav;

    private string _email = "";
    private string? _error;
    private bool _loading;
    private string _password = "";

    public Login(AuthService auth, NavigationManager nav)
    {
        _auth = auth;
        _nav = nav;
    }
    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }

    private async Task HandleLogin()
    {
        _loading = true;
        _error = null;

        var ok = await _auth.LoginAsync(new LoginRequest
        {
            Email = _email, Password = _password
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
