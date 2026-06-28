using FosterFlow.Contracts.DTOs.Auth;
using FosterFlow.Web.Services;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Signin.Components;

public partial class FosterSigninForm : ComponentBase
{
    private readonly AuthService _auth;

    private readonly RegisterFosterRequest _model = new();
    private readonly NavigationManager _nav;
    private readonly FluentValidation.IValidator<RegisterFosterRequest> _validator;
    private bool _loading;
    private string? _serverError;

    public FosterSigninForm(AuthService auth, NavigationManager nav, FluentValidation.IValidator<RegisterFosterRequest> validator)
    {
        _auth = auth;
        _nav = nav;
        _validator = validator;
    }

    // Re-evaluated on every render (i.e. after each field edit), so the submit
    // button enables only once the model satisfies the FluentValidation rules.
    private bool IsValid => _validator.Validate(_model).IsValid;

    // FosterFlowInput binds string only, so bridge to the int model field.
    private string MaxCatsInput
    {
        get => _model.MaxCats.ToString();
        set => _model.MaxCats = int.TryParse(value, out var n) ? n : 0;
    }

    // FosterFlowDateInput binds DateOnly?, so bridge to the non-nullable model fields.
    private DateOnly? AvailableFromInput
    {
        get => _model.AvailableFrom == default ? null : _model.AvailableFrom;
        set => _model.AvailableFrom = value ?? default;
    }

    private DateOnly? AvailableToInput
    {
        get => _model.AvailableTo == default ? null : _model.AvailableTo;
        set => _model.AvailableTo = value ?? default;
    }

    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }

    private async Task HandleLogin()
    {
        // EditForm only calls this when FluentValidation passes
        _loading = true;
        _serverError = null;

        /*var (success, error) = await _auth.RegisterShelterAsync(_model);
        if (!success)
        {
            _serverError = error ?? "Something went wrong. Please try again.";
            _loading = false;
            return;
        }*/
        _loading = false;
        _serverError = null;
        _nav.NavigateTo(ReturnUrl ?? "/");
    }
}
