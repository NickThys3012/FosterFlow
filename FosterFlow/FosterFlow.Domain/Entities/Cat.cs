using FosterFlow.Domain.Enums;
namespace FosterFlow.Domain.Entities;

//TODO: this is boilerplate code for the proj setup and will be changed throughout the development
public class Cat
{
    public Cat() {}
    public Cat(string name, bool dogFriendly, bool isUrgent, Sex sex, Guid shelterId, string age, List<string> temperamentTags, string photoUrl, string fosterDuration, string medicalNeeds)
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
    }

    public CatStatus Status { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool DogFriendly { get; set; }
    public bool IsUrgent { get; set; }
    public Sex Sex { get; set; }
    public string FosterDuration { get; set; } = string.Empty;
    public string MedicalNeeds { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public List<string> TemperamentTags { get; set; } = [];

    public string ShelterId { get; set; }
}
