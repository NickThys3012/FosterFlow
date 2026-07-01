using System.Security.Claims;
using FosterFlow.Contracts.DTOs.Cats;
using FosterFlow.Web.Services.HttpServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
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

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private IJSRuntime Js { get; set; } = default!;

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

    private async Task ReturnToCats()
    {
        try
        {
            // First, try to go back in browser history
            var navigated = await Js.InvokeAsync<bool>("navigation.goBack");
            if (navigated)
            {
                return;
            }
        }
        catch
        {
            // If JS interop fails, fall through to default navigation
        }

        // Fall back to role-based navigation
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        var destination = role switch
        {
            "Foster" => "/Foster/Dashboard",
            "Shelter" => "/Shelter/Dashboard",
            "Admin" => "/Admin/Dashboard",
            _ => "/"
        };

        _nav.NavigateTo(destination);
    }
}
