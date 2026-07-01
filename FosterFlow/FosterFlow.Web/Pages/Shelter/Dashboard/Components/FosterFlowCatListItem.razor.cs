using FosterFlow.Domain.Enums;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Shelter.Dashboard.Components;

public partial class FosterFlowCatListItem : ComponentBase
{
    private readonly NavigationManager _nav;
    public FosterFlowCatListItem(NavigationManager nav)
    {
        _nav = nav;
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
        Status == CatStatus.UpForFostering ? "btn btn-sm btn-outline" :
        "btn btn-sm btn-ghost";

    private string ActionButtonText =>
        Matches is > 0 ? $"Matches ({Matches})" :
        Status == CatStatus.UpForFostering ? "View" :
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
}
