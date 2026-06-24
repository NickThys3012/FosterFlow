using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
namespace FosterFlow.Web.Layout;

public partial class LoginDisplay : ComponentBase
{
    private readonly NavigationManager _navigation;
    public LoginDisplay(NavigationManager navigation)
    {
        _navigation = navigation;
    }

    private void BeginLogOut()
    {
        _navigation.NavigateToLogout("authentication/logout");
    }
}
