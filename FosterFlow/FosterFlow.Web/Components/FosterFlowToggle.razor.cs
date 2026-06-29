using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Components;

public partial class FosterFlowToggle : ComponentBase
{
    [Parameter] public bool Value { get; set; }
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string? SubLabel { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool ShowDivider { get; set; }

    private async Task Toggle()
    {
        if (!Disabled)
        {
            await ValueChanged.InvokeAsync(!Value);
        }
    }
}
