using FosterFlow.Web.Shared;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Components;

public partial class FosterFlowSelect<TValue> : ComponentBase
{

    private bool _open;
    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string Icon { get; set; } = "ti-chevron-down";
    [Parameter] public string Placeholder { get; set; } = "Select…";
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool UseNativeSelect { get; set; } = true;
    [Parameter] public string? Hint { get; set; }
    [Parameter] public string? ErrorMessage { get; set; }
    [Parameter] public List<FfDropdownOption> Options { get; set; } = [];
    private string Id { get; } = $"dd-{Guid.NewGuid():N}";
    private bool ShowError => !string.IsNullOrEmpty(ErrorMessage);
    private string ValidationCssClass => ShowError ? "modified invalid" : string.Empty;

    private string? SelectedLabel =>
        Options.FirstOrDefault(o => o.Value?.Equals(Value) ?? false)?.Label;

    private void ToggleOpen()
    {
        _open = !_open;
    }

    private async Task SelectOption(FfDropdownOption opt)
    {
        _open = false;
        if (opt.Value is TValue typed)
        {
            await ValueChanged.InvokeAsync(typed);
        }
    }

    private async Task OnNativeChange(ChangeEventArgs e)
    {
        var raw = e.Value?.ToString();
        if (string.IsNullOrEmpty(raw))
        {
            await ValueChanged.InvokeAsync(default);
            return;
        }

        // Handle string directly
        if (typeof(TValue) == typeof(string))
        {
            await ValueChanged.InvokeAsync((TValue)(object)raw);
            return;
        }

        // Handle enums
        if (typeof(TValue).IsEnum && Enum.TryParse(typeof(TValue), raw, out var parsed))
        {
            await ValueChanged.InvokeAsync((TValue)parsed);
            return;
        }

        // Handle int
        if (typeof(TValue) == typeof(int) && int.TryParse(raw, out var i))
        {
            await ValueChanged.InvokeAsync((TValue)(object)i);
        }
    }
}
