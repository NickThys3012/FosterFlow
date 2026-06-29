using FluentValidation;
using FosterFlow.Contracts.DTOs.Auth;
using FosterFlow.Domain.Enums;
using FosterFlow.Web.Services;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages;

public partial class Login : ComponentBase
{
    private readonly AuthService _auth;

    private readonly LoginRequest _model = new();
    private readonly NavigationManager _nav;
    private readonly IValidator<LoginRequest> _validator;
    private bool _loading;
    private string? _serverError;

    public Login(AuthService auth, NavigationManager nav, IValidator<LoginRequest> validator)
    {
        _auth = auth;
        _nav = nav;
        _validator = validator;
    }

    private bool IsValid => _validator.Validate(_model).IsValid;


    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }

    private async Task HandleLogin()
    {
        // EditForm only calls this when FluentValidation passes
        _loading = true;
        _serverError = null;

        var (success, error, role) = await _auth.LoginAsync(_model);
        if (!success)
        {
            _serverError = error ?? "Something went wrong. Please try again.";
            _loading = false;
            return;
        }
        _loading = false;
        _serverError = null;

        switch (role)
        {
            case UserRole.Admin:
                _nav.NavigateTo("/Admin/Dashboard");
                break;
            case UserRole.Shelter:
                _nav.NavigateTo("/Shelter/Dashboard");
                break;
            case UserRole.Foster:
                _nav.NavigateTo("/Foster/Dashboard");
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
