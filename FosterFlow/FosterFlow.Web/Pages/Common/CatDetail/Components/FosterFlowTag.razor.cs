using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Common.CatDetail.Components;

public partial class FosterFlowTag : ComponentBase
{
    [Parameter] public string Value { get; set; } = string.Empty;
}
