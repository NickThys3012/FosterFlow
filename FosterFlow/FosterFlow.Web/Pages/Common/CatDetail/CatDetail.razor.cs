using System.Security.Claims;
using FosterFlow.Contracts.DTOs.Cats;
using FosterFlow.Contracts.DTOs.Cats.UpdateCatStatus;
using FosterFlow.Domain.Enums;
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
    private bool _isAdminOrShelter;
    private bool _showModal;

    public CatDetail(NavigationManager nav, CatService cat)
    {
        _nav = nav;
        _cat = cat;
    }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;

    [Parameter] public Guid Id { get; set; }
    private string CatAgeText => $"{_catDto?.CatAge ?? 0} month{(_catDto?.CatAge == 1 ? "" : "s")}";
    private string FosterDurationText => $"{_catDto?.FosterDuration ?? 0} week{(_catDto?.FosterDuration == 1 ? "" : "s")}";

    private string ModalTitle => $"Are you sure you want to delete {(string.IsNullOrWhiteSpace(_catDto?.CatName) ? "this cat" : _catDto.CatName)}?";
    private string ModalConfirmText => $"Delete {(string.IsNullOrWhiteSpace(_catDto?.CatName) ? "this cat" : _catDto.CatName)}";

    protected override async Task OnInitializedAsync()
    {
        var cat = await _cat.GetCatByIdAsync(Id);
        if (cat != null)
        {
            _catDto = cat;
        }
        else
        {
            _catNotFound = true;
        }

        _isAdminOrShelter = await IsAdminOrShelterAsync();
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

    private async Task<bool> IsAdminOrShelterAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        return role is "Admin" or "Shelter";
    }

    private async Task HandleStatusChanged(CatStatus newStatus)
    {
        var previousStatus = _catDto!.CatStatus;

        var model = new UpdateCatStatusRequest
        {
            CatStatus = newStatus
        };

        var (success, error) = await _cat.UpdateCatStatusAsync(model, Id);
        if (!success)
        {
            _catDto.CatStatus = previousStatus;
            // Handle error (e.g., show a notification) Todo add popup
            Console.WriteLine($"Error updating cat status: {error}");
            return;
        }

        _catDto.CatStatus = newStatus;

        // Optionally, you can trigger a UI update or notify the parent component Todo add popup
        Console.WriteLine($"Cat status updated successfully: {newStatus}.");

        StateHasChanged();
    }
    private void ShowDeleteModal()
    {
        _showModal = true;
    }

    private async Task DeleteCat(bool confirmed)
    {
        if (confirmed)
        {
            // Call the service to delete the cat
            var (success, error) = await _cat.DeleteCatAsync(Id);
            if (!success)
            {
                // Handle error (e.g., show a notification) Todo add popup
                Console.WriteLine($"Error deleting cat: {error}");
                return;
            }
            // Navigate back to the cat list or another appropriate page
            await ReturnToCats();
        }
        _showModal = false;
    }
}
