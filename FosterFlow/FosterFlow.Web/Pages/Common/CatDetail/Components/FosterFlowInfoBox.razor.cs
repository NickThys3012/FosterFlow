using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Common.CatDetail.Components;

public partial class FosterFlowInfoBox : ComponentBase
{
    [Parameter]
    public required string Label { get; set; }

    [Parameter]
    public required string Value { get; set; }
}
