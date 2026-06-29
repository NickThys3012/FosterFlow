using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Pages.Shelter;

public partial class Dashboard : ComponentBase
{
    private readonly NavigationManager _nav;
    public Dashboard(NavigationManager nav) {
        _nav = nav;
    }
    private void CreateListing()
    {
        _nav.NavigateTo("/Shelter/CreateCatListing");
    }
}
