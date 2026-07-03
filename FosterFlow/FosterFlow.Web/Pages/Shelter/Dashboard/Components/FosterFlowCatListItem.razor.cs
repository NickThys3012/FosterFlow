using FosterFlow.Contracts.DTOs.Cats.UpdateCatStatus;
using FosterFlow.Domain.Enums;
using FosterFlow.Web.Services.HttpServices;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Shelter.Dashboard.Components;

public partial class FosterFlowCatListItem : ComponentBase
{
    private readonly CatService _catService;
    private readonly NavigationManager _nav;
    public FosterFlowCatListItem(NavigationManager nav, CatService catService)
    {
        _nav = nav;
        _catService = catService;
    }
    [Parameter] public Guid Id { get; set; }
    [Parameter] public string Name { get; set; } = string.Empty;
    [Parameter] public int Age { get; set; }
    [Parameter] public Sex Sex { get; set; }
    [Parameter] public int WeeksNeeded { get; set; }
    [Parameter] public bool IsUrgent { get; set; }
    [Parameter] public int? Matches { get; set; }
    [Parameter] public CatStatus Status { get; set; }
    [Parameter] public string? PhotoUrl { get; set; }

    private string MetaText => $"{FormatAge(Age)} · {Sex} · {WeeksNeeded} weeks needed";

    private string ActionButtonClass =>
        Matches is > 0 ? "btn btn-sm" :
        Status == CatStatus.Available ? "btn btn-sm btn-outline" :
        "btn btn-sm btn-ghost";

    private string ActionButtonText =>
        Matches is > 0 ? $"Matches ({Matches})" :
        Status == CatStatus.Available ? "View" :
        "Details";

    private static string FormatAge(int ageInMonths)
    {
        if (ageInMonths < 12)
        {
            return $"{ageInMonths} {(ageInMonths == 1 ? "month" : "months")}";
        }

        var years = ageInMonths / 12;
        return $"{years} {(years == 1 ? "year" : "years")}";
    }
    private void RedirectToCatDetail()
    {
        _nav.NavigateTo($"/Cat/{Id}");
    }
    private void RedirectToCatUpdate()
    {
        _nav.NavigateTo($"/Shelter/Cat/{Id}/Update");
    }

    private async Task UpdateCatStatusAsync(CatStatus status)
    {
        Status = status;
        var model = new UpdateCatStatusRequest
        {
            CatStatus = status
        };
        var (success, error) = await _catService.UpdateCatStatusAsync(model, Id);
        if (!success)
        {
            // Handle error (e.g., show a notification) Todo add popup
            Console.WriteLine($"Error updating cat status: {error}");
            return;
        }

        // Optionally, you can trigger a UI update or notify the parent component Todo add popup
        Console.WriteLine($"Cat {nameof(status)} updated successfully.");

        StateHasChanged();
    }
}
