using FosterFlow.Domain.Enums;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Shelter.Dashboard.Components;

public partial class FosterFlowCatListItem : ComponentBase
{
    [Parameter] public string Name { get; set; } = string.Empty;
    [Parameter] public int Age { get; set; }
    [Parameter] public Sex Sex { get; set; }
    [Parameter] public int WeeksNeeded { get; set; }
    [Parameter] public bool IsUrgent { get; set; }
    [Parameter] public int? Matches { get; set; }
    [Parameter] public CatStatus Status { get; set; }
    [Parameter] public string? PhotoUrl { get; set; }

    private string MetaText => $"{FormatAge(Age)} · {Sex} · {WeeksNeeded} weeks needed";

    private string StatusBadgeClass =>
        Status switch
        {
            CatStatus.UpForFostering => "badge badge-available",
            CatStatus.Pending => "badge badge-pending",
            CatStatus.Matched => "badge badge-matched",
            _ => string.Empty
        };

    private string StatusBadgeText =>
        Status switch
        {
            CatStatus.UpForFostering => "Available",
            CatStatus.Pending => "Pending",
            CatStatus.Matched => "Matched",
            _ => string.Empty
        };

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
}
