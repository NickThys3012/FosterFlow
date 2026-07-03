using FosterFlow.Domain.Enums;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Components;

public partial class FosterFlowBadge : ComponentBase
{
    [Parameter] public bool IsUrgent { get; set; }
    [Parameter] public CatStatus Status { get; set; }
    [Parameter] public string CustomBadgeText { get; set; } = string.Empty;
    [Parameter] public string CustomBadgeClass { get; set; } = string.Empty;

    private string StatusBadgeClass()
    {
        if (!string.IsNullOrWhiteSpace(CustomBadgeClass))
        {
            return CustomBadgeClass;
        }
        if (IsUrgent)
        {
            return "badge-urgent";
        }

        return Status switch
        {
            CatStatus.Available => "badge-available",
            CatStatus.Pending => "badge-pending",
            CatStatus.Matched => "badge-matched",
            CatStatus.Deceased => "badge-deceased",
            CatStatus.InFoster => "badge-in-foster",
            CatStatus.Returned => "badge-returned",
            CatStatus.Adopted => "badge-adopted",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private string StatusBadgeText()
    {
        if (!string.IsNullOrWhiteSpace(CustomBadgeText))
        {
            return CustomBadgeText;
        }
        if (IsUrgent)
        {
            return "⚡ Urgent";
        }
        return Status switch
        {
            CatStatus.Available => "Available",
            CatStatus.Pending => "Pending",
            CatStatus.Matched => "Matched",
            CatStatus.Deceased => "Deceased",
            CatStatus.InFoster => "In Foster",
            CatStatus.Returned => "Returned",
            CatStatus.Adopted => "Adopted",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
