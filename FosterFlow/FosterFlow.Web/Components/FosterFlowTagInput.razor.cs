using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FosterFlow.Web.Components;

public partial class FosterFlowTagInput : ComponentBase
{
    private bool _isAdding;
    private string _draft = string.Empty;

    [Parameter] public List<string> Values { get; set; } = new();
    [Parameter] public EventCallback<List<string>> ValuesChanged { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? Hint { get; set; }
    [Parameter] public string Placeholder { get; set; } = "New tag";

    private void BeginAdd()
    {
        _isAdding = true;
        _draft = string.Empty;
    }

    private async Task AddTag()
    {
        var value = _draft.Trim();
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        if (Values.Any(tag => string.Equals(tag, value, StringComparison.OrdinalIgnoreCase)))
        {
            CancelAdd();
            return;
        }

        var updated = Values.ToList();
        updated.Add(value);
        Values = updated;
        _draft = string.Empty;
        _isAdding = false;
        await ValuesChanged.InvokeAsync(updated);
    }

    private async Task RemoveTag(string tag)
    {
        var updated = Values.Where(current => !string.Equals(current, tag, StringComparison.OrdinalIgnoreCase)).ToList();
        Values = updated;
        await ValuesChanged.InvokeAsync(updated);
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await AddTag();
        }
        else if (e.Key == "Escape")
        {
            CancelAdd();
        }
    }

    private void CancelAdd()
    {
        _isAdding = false;
        _draft = string.Empty;
    }
}
