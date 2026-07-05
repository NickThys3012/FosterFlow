using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Components;

public partial class FosterFlowModal : ComponentBase
{
    [Parameter] [EditorRequired]
    public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public string? ConfirmText { get; set; } = "Confirm";
    [Parameter] public EventCallback<bool> ConfirmEvent { get; set; }

    private async Task CancelConfirm()
    {
        await ConfirmEvent.InvokeAsync(false);
    }

    private async Task ConfirmModal()
    {
        await ConfirmEvent.InvokeAsync(true);
    }
}
