using FosterFlow.Domain.Enums;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Components;

public partial class FosterFlowStatusControl : ComponentBase
{

    private static readonly CatStatus[] AllStatuses =
    [
        CatStatus.Available,
        CatStatus.Pending,
        CatStatus.InFoster,
        CatStatus.Returned,
        CatStatus.Adopted,
        CatStatus.Deceased
    ];

    private static readonly HashSet<CatStatus> StatusesRequiringConfirmation =
    [
        CatStatus.Adopted,
        CatStatus.Deceased
    ];

    private bool _menuOpen;
    private CatStatus? _pendingStatus;
    private bool _saving;
    [Parameter] [EditorRequired]
    public CatStatus CurrentStatus { get; set; }

    /// <summary>Optional cat name, used to personalize the confirmation copy.</summary>
    [Parameter]
    public string? CatName { get; set; }

    /// <summary>Disables the control (e.g. while a parent-level operation is in flight).</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    ///     Raised when the user picks a new status and (if required) confirms it.
    ///     The parent is expected to call the API here; this component stays in a
    ///     "Saving…" state until the awaited callback completes.
    /// </summary>
    [Parameter]
    public EventCallback<CatStatus> OnStatusChanged { get; set; }

    private string ModalTitle
    {
        get
        {
            return _pendingStatus != null ? $"Mark {(string.IsNullOrWhiteSpace(CatName) ? "this cat" : CatName)} as {GetLabel(_pendingStatus.Value)}?" : string.Empty;
        }
    }
    private string ModalConfirmText
    {
        get
        {
            return _pendingStatus != null ? $"Yes, mark {(string.IsNullOrWhiteSpace(CatName) ? "this cat" : CatName)} as {GetLabel(_pendingStatus.Value)}?" : string.Empty;
        }
    }

    private void ToggleMenu()
    {
        if (Disabled || _saving)
        {
            return;
        }
        _menuOpen = !_menuOpen;
    }

    private void CloseMenu()
    {
        _menuOpen = false;
    }

    private async Task SelectStatus(CatStatus status)
    {
        _menuOpen = false;

        if (status == CurrentStatus)
        {
            return;
        }

        if (StatusesRequiringConfirmation.Contains(status))
        {
            _pendingStatus = status;
            return;
        }

        await ApplyStatusChange(status);
    }

    private void CancelConfirm()
    {
        _pendingStatus = null;
    }
    private async Task ModalCallback(bool confirmed)
    {
        if (confirmed)
        {
            await ConfirmPendingStatus();
        }
        else
        {
            CancelConfirm();
        }
    }
    private async Task ConfirmPendingStatus()
    {
        if (_pendingStatus is null)
        {
            return;
        }
        var status = _pendingStatus.Value;
        _pendingStatus = null;
        await ApplyStatusChange(status);
    }

    private async Task ApplyStatusChange(CatStatus status)
    {
        _saving = true;
        try
        {
            if (OnStatusChanged.HasDelegate)
            {
                await OnStatusChanged.InvokeAsync(status);
            }
        }
        finally
        {
            _saving = false;
        }
    }

    private static string GetLabel(CatStatus status)
    {
        return status switch
        {
            CatStatus.Available => "Available",
            CatStatus.Matched => "Matched",
            CatStatus.InFoster => "In Foster",
            CatStatus.Returned => "Returned",
            CatStatus.Adopted => "Adopted",
            CatStatus.Deceased => "Deceased",
            CatStatus.Pending => "Pending",
            _ => status.ToString()
        };
    }

    private static string GetBadgeClass(CatStatus status)
    {
        return status switch
        {
            CatStatus.Available => "ff-status--available",
            CatStatus.Matched => "ff-status--matched",
            CatStatus.InFoster => "ff-status--infoster",
            CatStatus.Returned => "ff-status--returned",
            CatStatus.Adopted => "ff-status--adopted",
            CatStatus.Deceased => "ff-status--deceased",
            CatStatus.Pending => "ff-status--pending",
            _ => ""
        };
    }
}
