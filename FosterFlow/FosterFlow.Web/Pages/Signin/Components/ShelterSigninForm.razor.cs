using System.ComponentModel.DataAnnotations;
using FosterFlow.Contracts.DTOs.Auth;
using Microsoft.AspNetCore.Components;

namespace FosterFlow.Web.Pages.Signin.Components;

public partial class ShelterSigninForm : ComponentBase
{
    private readonly RegisterShelterRequest _model = new();
    private string?      _serverError;
    private bool         _loading;

    private async Task HandleLogin()
    {
        // EditForm only calls this when FluentValidation passes
        _loading     = true;
        _serverError = null;
        Thread.Sleep(1000);
        
        _loading     = false;
        _serverError = null;
    }

}
