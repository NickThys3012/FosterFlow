using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Components;

public partial class FosterFlowModal : ComponentBase
{
    [Parameter] [EditorRequired]
    public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public string? ConfirmText { get; set; } = "Confirm";
    [Parameter] public EventCallback<bool> ConfirmEvent { get; set; }

    private void CancelConfirm()
    {
        ConfirmEvent.InvokeAsync(false);
    }
    private void ConfirmModal()
    {
        ConfirmEvent.InvokeAsync(true);
    }
}
