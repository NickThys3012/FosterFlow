using System.Security.Claims;
using FosterFlow.Contracts.DTOs.Cats.GetAllCats;
using FosterFlow.Domain.Enums;
using FosterFlow.Web.Services.HttpServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
namespace FosterFlow.Web.Pages.Shelter.Dashboard;

public partial class Dashboard : ComponentBase
{
    private readonly CatService _catService;
    private readonly NavigationManager _nav;
    private GetAllCatsResponse _cats = new();
    protected string? UserName { get; private set; }

    public Dashboard(NavigationManager nav, CatService catService)
    {
        _nav = nav;
        _catService = catService;
    }

    public List<GetAllCatsDto> PendingCats => _cats.Cats.Where(c => c.Status == CatStatus.Pending).ToList();
    public List<GetAllCatsDto> MatchedCats => _cats.Cats.Where(c => c.Status == CatStatus.Matched ).ToList();
    private void CreateListing()
    {
        _nav.NavigateTo("/Shelter/CreateCatListing");
    }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        UserName = authState.User.Identity?.Name
            ?? authState.User.FindFirst(ClaimTypes.Email)?.Value
            ?? "there";

        var result = await _catService.GetAllCatsAsync();
        if (result != null)
        {
            _cats = result;
        }
    }

    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
}
