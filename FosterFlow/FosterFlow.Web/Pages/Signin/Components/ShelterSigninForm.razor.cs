using FosterFlow.Contracts.DTOs.Auth;
using FosterFlow.Contracts.Validators.Auth;
using FosterFlow.Web.Services;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Signin.Components;

public partial class ShelterSigninForm : ComponentBase
{
    private readonly AuthService _auth;

    private readonly RegisterShelterRequest _model = new();
    private readonly NavigationManager _nav;
    private readonly RegisterShelterRequestValidator _validator = new();
    private bool _loading;
    private string? _serverError;
    public ShelterSigninForm(AuthService auth, NavigationManager nav)
    {
        _auth = auth;
        _nav = nav;
    }

    // Re-evaluated on every render (i.e. after each field edit), so the submit
    // button enables only once the model satisfies the FluentValidation rules.
    private bool IsValid => _validator.Validate(_model).IsValid;

    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }

    private async Task HandleLogin()
    {
        // EditForm only calls this when FluentValidation passes
        _loading = true;
        _serverError = null;

        var (success, error) = await _auth.RegisterShelterAsync(_model);
        if (!success)
        {
            _serverError = error ?? "Something went wrong. Please try again.";
            _loading = false;
            return;
        }
        _loading = false;
        _serverError = null;
        _nav.NavigateTo(ReturnUrl ?? "/");
    }
}
