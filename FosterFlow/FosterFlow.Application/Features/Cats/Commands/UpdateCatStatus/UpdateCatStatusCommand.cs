using FosterFlow.Contracts.DTOs.Cats.UpdateCatStatus;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.UpdateCatStatus;

public record UpdateCatStatusCommand(UpdateCatStatusRequest Request, Guid CatId) : IRequest;
