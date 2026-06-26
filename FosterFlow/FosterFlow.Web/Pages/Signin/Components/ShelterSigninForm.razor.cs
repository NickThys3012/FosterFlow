using FosterFlow.Contracts.DTOs.Auth;
using FosterFlow.Contracts.Validators.Auth;
using FosterFlow.Web.Services;
using Microsoft.AspNetCore.Components;

namespace FosterFlow.Web.Pages.Signin.Components;

public partial class ShelterSigninForm : ComponentBase
{
    private readonly AuthService _auth;
    private readonly NavigationManager _nav;

    private readonly RegisterShelterRequest _model = new();
    private readonly RegisterShelterRequestValidator _validator = new();
    private string?      _serverError;
    private bool         _loading;

    // Re-evaluated on every render (i.e. after each field edit), so the submit
    // button enables only once the model satisfies the FluentValidation rules.
    private bool IsValid => _validator.Validate(_model).IsValid;
    public ShelterSigninForm(AuthService auth, NavigationManager nav)
    {
        _auth = auth;
        _nav = nav;
    }

    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }

    private async Task HandleLogin()
    {
        // EditForm only calls this when FluentValidation passes
        _loading     = true;
        _serverError = null;
        
        var success = await _auth.RegisterShelterAsync(_model);
        if (!success)
        {
            _serverError = "Something went wrong. Please try again.";
            _loading = false;
            return;
        }
        _loading     = false;
        _serverError = null;
        _nav.NavigateTo(ReturnUrl ?? "/");

    }

}
