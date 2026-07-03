using FosterFlow.Domain.Entities.Base;
using FosterFlow.Domain.Enums;
namespace FosterFlow.Domain.Entities;

public class Cat : BaseEntity
{
    public Cat() {}
    public Cat(string name, bool dogFriendly, bool isUrgent, Sex sex, Guid shelterId, int age, List<string> temperamentTags, string photoUrl, int fosterDuration, string medicalNeeds)
    {
        Name = name;
        DogFriendly = dogFriendly;
        IsUrgent = isUrgent;
        Sex = sex;
        ShelterId = shelterId.ToString();
        Age = age;
        TemperamentTags = temperamentTags;
        PhotoUrl = photoUrl;
        FosterDuration = fosterDuration;
        MedicalNeeds = medicalNeeds;
        Status = CatStatus.Available;
    }

    public CatStatus Status { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool DogFriendly { get; set; }
    public bool IsUrgent { get; set; }
    public Sex Sex { get; set; }
    public int FosterDuration { get; set; }
    public string MedicalNeeds { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public int Age { get; set; }
    public List<string> TemperamentTags { get; set; } = [];
    public string ShelterId { get; set; } = string.Empty;

    public bool IsArchived { get; set; }
    public bool IsDeleted { get; set; }
}
