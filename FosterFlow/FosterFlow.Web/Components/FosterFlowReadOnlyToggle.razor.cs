using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Components;

public partial class FosterFlowReadOnlyToggle : ComponentBase
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool Value { get; set; }
    [Parameter] public string TrueText { get; set; } = "Yes";
    [Parameter] public string FalseText { get; set; } = "No";

    private string DisplayValue => Value ? TrueText : FalseText;
}
