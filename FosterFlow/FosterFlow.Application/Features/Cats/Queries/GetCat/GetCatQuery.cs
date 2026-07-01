using FosterFlow.Contracts.DTOs.Cats;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Queries.GetCat;

public record GetCatQuery(Guid CatId) : IRequest<CatDto>;
