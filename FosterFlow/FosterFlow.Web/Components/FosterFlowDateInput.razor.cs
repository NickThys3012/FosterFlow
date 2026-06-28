using Microsoft.AspNetCore.Components;

namespace FosterFlow.Web.Components;

public partial class FosterFlowDateInput : ComponentBase
{
    [Parameter] public DateOnly? Value { get; set; }
    [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string Icon { get; set; } = "ti-calendar";
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? Hint { get; set; }
    [Parameter] public string? ErrorMessage { get; set; }
    [Parameter] public DateOnly? Min { get; set; }
    [Parameter] public DateOnly? Max { get; set; }

    private static string Id => $"date-{Guid.NewGuid():N}";

    private string ValueStr => Value?.ToString("yyyy-MM-dd") ?? string.Empty;
    private string MinStr => Min?.ToString("yyyy-MM-dd") ?? string.Empty;
    private string MaxStr => Max?.ToString("yyyy-MM-dd") ?? string.Empty;
    private bool ShowError => !string.IsNullOrEmpty(ErrorMessage);

    private string CssClass => ShowError ? "modified invalid" : string.Empty;

    private async Task OnChange(ChangeEventArgs e)
    {
        if (DateOnly.TryParse(e.Value?.ToString(), out var parsed))
            await ValueChanged.InvokeAsync(parsed);
        else
            await ValueChanged.InvokeAsync(null);
    }
}

