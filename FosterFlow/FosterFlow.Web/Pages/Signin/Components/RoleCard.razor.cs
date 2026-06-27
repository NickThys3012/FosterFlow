using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
namespace FosterFlow.Web.Pages.Signin.Components;

public partial class RoleCard : ComponentBase
{
    [Parameter] public string Icon { get; set; } = "🐾";
    [Parameter] public string Name { get; set; } = "Name";
    [Parameter] public string Description { get; set; } = "Description";
    [Parameter] public bool Selected { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> CardClickedEvent { get; set; }

    private Task CardClicked(MouseEventArgs args)
    {
        return CardClickedEvent.InvokeAsync(args);
    }
}
