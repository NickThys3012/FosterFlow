using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.DeleteCat;

public record DeleteCatCommand(Guid CatId) : IRequest;
