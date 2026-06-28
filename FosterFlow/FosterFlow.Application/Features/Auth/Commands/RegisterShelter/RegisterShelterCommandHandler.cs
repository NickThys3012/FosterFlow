using FluentValidation.Results;
using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Auth.Commands.RegisterShelter;

public class RegisterShelterCommandHandler : IRequestHandler<RegisterShelterCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IBusinessMetrics _metrics;
    private readonly IUserRepository _userRepository;
    public RegisterShelterCommandHandler(IIdentityService identityService, IUserRepository userRepository, IBusinessMetrics metrics)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _metrics = metrics;
    }
    public async Task Handle(RegisterShelterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Cmd.Email);
        if (existing is not null)
        {
            throw new ValidationException([
                new ValidationFailure("Email", "Email already exists")
            ]);
        }

        await _identityService.RegisterShelterAsync(request.Cmd.Email, request.Cmd.Password, request.Cmd.Name, request.Cmd.Phone!, request.Cmd.Street!, request.Cmd.PostalCode!,
            request.Cmd.City!, request.Cmd.Country!);

        _metrics.IncrementActiveShelters();
    }
}
