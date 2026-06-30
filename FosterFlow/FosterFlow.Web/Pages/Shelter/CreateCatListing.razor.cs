using FluentValidation;
using FosterFlow.Contracts.DTOs.Cats;
using FosterFlow.Web.Services.HttpServices;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Shelter;

public partial class CreateCatListing : ComponentBase
{
    private readonly CatService _cat;
    private readonly CreateCatRequest _model = new();
    private readonly NavigationManager _nav;


    private readonly IValidator<CreateCatRequest> _validator;

    private bool _loading;
    private string? _serverError;
    public CreateCatListing(IValidator<CreateCatRequest> validator, NavigationManager nav, CatService cat)
    {
        _validator = validator;
        _nav = nav;
        _cat = cat;
    }

    private bool IsValid => _validator.Validate(_model).IsValid;

    // FosterFlowInput binds string only, so bridge to the int model field.
    private string AgeInput
    {
        get => _model.Age.ToString();
        set => _model.Age = int.TryParse(value, out var n) ? n : 0;
    }    
    
    // FosterFlowInput binds string only, so bridge to the int model field.
    private string FosterDurationInput
    {
        get => _model.FosterDuration.ToString();
        set => _model.FosterDuration = int.TryParse(value, out var n) ? n : 0;
    }
    
    // on create listing 
    // first upload the photo
    // then create the listing
    private async Task HandleCreateCat()
    {
        // EditForm only calls this when FluentValidation passes
        _loading = true;
        _serverError = null;

        var (success, error, _) = await _cat.CreateCatAsync(_model);
        if (!success)
        {
            _serverError = error ?? "Something went wrong. Please try again.";
            _loading = false;
            return;
        }
        _loading = false;
        _serverError = null;

        _nav.NavigateTo("/Shelter/Dashboard");
    }
}
