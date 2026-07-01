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
            CatStatus.UpForFostering => "badge-available",
            CatStatus.Pending => "badge-pending",
            CatStatus.Matched => "badge-matched",
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
            CatStatus.UpForFostering => "Available",
            CatStatus.Pending => "Pending",
            CatStatus.Matched => "Matched",

            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
