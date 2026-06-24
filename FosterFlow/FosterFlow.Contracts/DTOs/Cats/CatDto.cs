using FosterFlow.Domain.Enums;
namespace FosterFlow.Contracts.DTOs.Cats;

public class CatDto
{
    public CatStatus Status { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
}
