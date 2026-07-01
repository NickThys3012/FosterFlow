using FosterFlow.Domain.Enums;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Shelter.Dashboard.Components;

public partial class FosterFlowPendingCard : ComponentBase
{
    [Parameter] public string FostersName { get; set; } = string.Empty;
    [Parameter] public string CatName { get; set; } = string.Empty;
    [Parameter] public ExperienceLevel Experience { get; set; }
    [Parameter] public HomeType HomeType { get; set; }

    [Parameter] public bool HasDogs { get; set; }

    [Parameter] public bool IsUrgent { get; set; }
    public string DogField => HasDogs ? "Has dogs" : "No dogs";
    public string UrgentField => IsUrgent ? "Urgent" : "Not urgent";
}
