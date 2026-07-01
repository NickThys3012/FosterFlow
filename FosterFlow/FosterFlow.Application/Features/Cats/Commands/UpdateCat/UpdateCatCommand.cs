using FosterFlow.Contracts.DTOs.Cats.UpdateCat;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.UpdateCat;

public record UpdateCatCommand(UpdateCatRequest Request, Guid ShelterId) : IRequest;
