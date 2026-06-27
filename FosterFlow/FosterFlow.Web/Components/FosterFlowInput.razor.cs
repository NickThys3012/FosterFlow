using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Components;

public partial class FosterFlowInput : ComponentBase
{
    [Parameter] public string Label { get; set; } = "";
    [Parameter] public bool Required { get; set; } = true;
    [Parameter] public string Type { get; set; } = "text";
    [Parameter] public bool ReadOnly { get; set; }

    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public Expression<Func<string?>>? ValueExpression { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
