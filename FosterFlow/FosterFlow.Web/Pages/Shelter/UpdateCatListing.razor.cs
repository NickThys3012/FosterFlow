using FluentValidation;
using FosterFlow.Contracts.DTOs.Cats.UpdateCat;
using FosterFlow.Web.Services.HttpServices;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Shelter;

public partial class UpdateCatListing : ComponentBase
{

    private readonly CatService _cat;
    private readonly UpdateCatRequest _model = new();
    private readonly NavigationManager _nav;


    private readonly IValidator<UpdateCatRequest> _validator;


    private bool _loading;
    private string? _serverError;

    public UpdateCatListing(CatService cat, NavigationManager nav, IValidator<UpdateCatRequest> validator)
    {
        _cat = cat;
        _nav = nav;
        _validator = validator;
    }
    [Parameter] public Guid Id { get; set; }

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

    private bool HasPendingMatches { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var cat = await _cat.GetCatByIdAsync(Id);
        if (cat != null)
        {
            _model.Id = cat.Id;
            _model.Name = cat.CatName;
            _model.DogFriendly = cat.DogFriendly;
            _model.IsUrgent = cat.IsUrgent;
            _model.Sex = cat.CatSex;
            _model.PhotoUrl = cat.CatPhotoUrl;
            _model.Age = cat.CatAge;
            _model.FosterDuration = cat.FosterDuration;
            _model.MedicalNeeds = cat.MedicalNeeds;
            _model.TemperamentTags = cat.TemperamentTags;
            
            HasPendingMatches = cat.CatStatus == Domain.Enums.CatStatus.Pending;
        }
    }

    private async Task HandleUpdateCat()
    {
        // EditForm only calls this when FluentValidation passes
        _loading = true;
        _serverError = null;

        var (success, error) = await _cat.UpdateCatAsync(_model);
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
