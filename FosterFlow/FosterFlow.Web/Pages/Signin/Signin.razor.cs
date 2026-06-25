using FosterFlow.Domain.Enums;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Signin;

public partial class Signin : ComponentBase
{
    private UserRole? _role;
    
    private void HandleRoleChange(UserRole role)
    {
        _role = role;
    }
}
