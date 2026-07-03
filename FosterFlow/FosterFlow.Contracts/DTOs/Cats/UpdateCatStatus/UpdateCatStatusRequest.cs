using FosterFlow.Domain.Enums;
namespace FosterFlow.Contracts.DTOs.Cats.UpdateCatStatus;

public class UpdateCatStatusRequest
{
    public CatStatus CatStatus { get; set; }
}
