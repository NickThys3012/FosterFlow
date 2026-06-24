using FosterFlow.Contracts.DTOs.Cats;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Queries.GetCats;

public class GetCatQuery(Guid id) : IRequest<CatDto>
{
    public Guid Id { get; } = id;
}
