using FosterFlow.Contracts.DTOs.Cats.CreateCat;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.CreateCat;

public record CreateCatCommand(CreateCatRequest Request) : IRequest<Guid>;
