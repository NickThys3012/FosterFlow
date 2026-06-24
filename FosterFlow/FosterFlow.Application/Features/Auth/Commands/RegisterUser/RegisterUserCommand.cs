using FosterFlow.Contracts.DTOs.Auth;
using MediatR;
namespace FosterFlow.Application.Features.Auth.Commands.RegisterUser;

public record RegisterUserCommand(RegisterUserRequest Request) : IRequest;
