using FosterFlow.Contracts.DTOs.Auth;
using MediatR;
namespace FosterFlow.Application.Features.Auth.Commands.RegisterShelter;

public record RegisterShelterCommand(RegisterShelterRequest Request) : IRequest;
