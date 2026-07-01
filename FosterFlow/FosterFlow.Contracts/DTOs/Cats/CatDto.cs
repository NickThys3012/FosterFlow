using FosterFlow.Domain.Enums;
namespace FosterFlow.Contracts.DTOs.Cats;

public class CatDto
{
    public Guid Id { get; set; }
    public int CatAge { get; set; } = 6;
    public string CatName { get; set; } = "rajah";
    public string CatPhotoUrl { get; set; } = string.Empty;
    public Sex CatSex { get; set; }
    public CatStatus CatStatus { get; set; }
    public int FosterDuration { get; set; }
    public bool DogFriendly { get; set; } = true;
    public string ShelterName { get; set; } = "Shelter";
    public string ShelterLocation { get; set; } = "Location";
    public string MedicalNeeds { get; set; } = string.Empty;
    public List<string> TempramentTags { get; set; } = [];
}
