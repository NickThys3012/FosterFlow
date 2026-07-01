using FosterFlow.Domain.Enums;
namespace FosterFlow.Contracts.DTOs.Cats;

public class CatDto
{
    public Guid Id { get; set; }
    public int CatAge { get; set; }
    public string CatName { get; set; } = string.Empty;
    public string CatPhotoUrl { get; set; } = string.Empty;
    public Sex CatSex { get; set; }
    public CatStatus CatStatus { get; set; }
    public int FosterDuration { get; set; }
    public bool DogFriendly { get; set; } = true;
    public string ShelterName { get; set; } = string.Empty;
    public string ShelterLocation { get; set; } = string.Empty;
    public string MedicalNeeds { get; set; } = string.Empty;
    public List<string> TemperamentTags { get; set; } = [];
    public bool IsUrgent { get; set; }
}
