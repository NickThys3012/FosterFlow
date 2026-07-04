using FosterFlow.Contracts.DTOs.Cats.GetAllCats;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Queries.GetAllCats;

public record GetAllCatsQuery : IRequest<GetAllCatsResponse>;
