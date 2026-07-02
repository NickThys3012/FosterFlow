using FosterFlow.Web.Services.HttpServices;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Layout;

public partial class LoginDisplay : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    [Inject]
    private AuthService AuthService { get; set; } = null!;

    private async Task BeginLogOut()
    {
        await AuthService.LogoutAsync();
        Navigation.NavigateTo("/");
    }
}
