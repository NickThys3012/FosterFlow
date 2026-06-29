using Microsoft.AspNetCore.Components;
using FosterFlow.Domain.Enums;

namespace FosterFlow.Web.Pages.Shelter;

public partial class CreateCatListing : ComponentBase
{
    private bool _dogFriendly = true;
    private bool _isUrgent;
    private Sex _sex = Sex.Female;
    private string? _fosterDuration;
    private string? _medicalNeeds;
    private string? _photoUrl;
    private string? _catName;
    private string? _age;
    private List<string> _temperamentTags = ["Shy", "Indoor", "Kid-friendly"];
}
