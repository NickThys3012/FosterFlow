using FosterFlow.Contracts.DTOs.Auth;
using MediatR;
namespace FosterFlow.Application.Features.Auth.Commands.RegisterFoster;

public record RegisterFosterCommand(RegisterFosterRequest Cmd) : IRequest;
