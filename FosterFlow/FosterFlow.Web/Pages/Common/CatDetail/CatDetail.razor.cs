using FosterFlow.Contracts.DTOs.Cats;
using FosterFlow.Web.Services.HttpServices;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Common.CatDetail;

public partial class CatDetail : ComponentBase
{
    private readonly CatService _cat;
    private readonly NavigationManager _nav;
    private CatDto? _catDto;

    private bool _catNotFound;
    public CatDetail(NavigationManager nav, CatService cat)
    {
        _nav = nav;
        _cat = cat;
    }

    [Parameter] public Guid Id { get; set; }
    private string CatAgeText => $"{_catDto?.CatAge ?? 0} month{(_catDto?.CatAge == 1 ? "" : "s")}";
    private string FosterDurationText => $"{_catDto?.FosterDuration ?? 0} week{(_catDto?.FosterDuration == 1 ? "" : "s")}";

    protected override async Task OnInitializedAsync()
    {
        var cat = await _cat.GetCatByIdAsync(Id);
        if (cat != null)
        {
            _catDto = cat;
            return;
        }
        _catNotFound = true;
    }

    private void ReturnToCats()
    {
        _nav.NavigateTo("/Shelter/Dashboard");
    }
}
