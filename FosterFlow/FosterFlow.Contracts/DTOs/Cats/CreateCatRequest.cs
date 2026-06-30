using FosterFlow.Domain.Enums;
namespace FosterFlow.Contracts.DTOs.Cats;

public class CreateCatRequest
{
    public string Name { get; set; }
    public bool DogFriendly { get; set; }
    public bool IsUrgent { get; set; }
    public Sex Sex { get; set; }
    public string FosterDuration { get; set; } = string.Empty;
    public string MedicalNeeds { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public List<string> TemperamentTags { get; set; } =
    [
        "Shy", "Indoor", "Kid-friendly"
    ];
}
