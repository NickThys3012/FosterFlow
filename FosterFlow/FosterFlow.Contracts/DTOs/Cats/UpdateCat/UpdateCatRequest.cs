using FosterFlow.Domain.Enums;
namespace FosterFlow.Contracts.DTOs.Cats.UpdateCat;

public class UpdateCatRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool DogFriendly { get; set; }
    public bool IsUrgent { get; set; }
    public Sex Sex { get; set; }
    /// <summary>
    ///     Foster duration in weeks
    /// </summary>
    public int FosterDuration { get; set; }
    public string MedicalNeeds { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    /// <summary>
    ///     Age in months
    /// </summary>
    public int Age { get; set; }
    public List<string> TemperamentTags { get; set; } = [];
}
