using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Shelter.Dashboard.Components;

public partial class FosterFlowStatCard : ComponentBase
{
    [Parameter]
    public int StatNum { get; set; }

    [Parameter]
    public string StatLabel { get; set; } = string.Empty;

    [Parameter]
    public string? StatColor { get; set; }

    private string? StatNumStyle => string.IsNullOrWhiteSpace(StatColor) ? null : $"color: {StatColor};";
}
