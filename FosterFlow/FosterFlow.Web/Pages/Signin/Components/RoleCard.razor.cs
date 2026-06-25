using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Signin.Components;

public partial class RoleCard : ComponentBase
{
    [Parameter] public string Icon { get; set; } = "🐾";
    [Parameter] public string Name { get; set; } = "Name";
    [Parameter] public string Description { get; set; } = "Description";
    [Parameter] public bool Selected { get; set; }
    [Parameter] public EventCallback CardClickedEvent { get; set; }
        private async Task CardClicked()
        {
            await CardClickedEvent.InvokeAsync();
        }
    
}

