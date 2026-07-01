using FosterFlow.Domain.Enums;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Components;

public partial class FosterFlowBadge : ComponentBase
{
    [Parameter] public bool IsUrgent { get; set; }
    [Parameter] public CatStatus Status { get; set; }

    private string StatusBadgeClass =>
        IsUrgent ? "badge-urgent" :
            Status switch
            {
                CatStatus.UpForFostering => "badge-available",
                CatStatus.Pending => "badge-pending",
                CatStatus.Matched => "badge-matched",
                _ => string.Empty
            };

    private string StatusBadgeText =>
        IsUrgent ? "⚡ Urgent" :
            Status switch
            {
                CatStatus.UpForFostering => "Available",
                CatStatus.Pending => "Pending",
                CatStatus.Matched => "Matched",
                _ => string.Empty
            };
}
