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
        var existing = await _userRepository.GetByEmailAsync(request.Request.Email);
        if (existing is not null)
        {
            throw new ValidationException([
                new ValidationFailure("Email", "Email already exists")
            ]);
        }

        await _identityService.RegisterShelterAsync(request.Request.Email, request.Request.Password, request.Request.Name, request.Request.Phone!, request.Request.Street!, request.Request.PostalCode!,
            request.Request.City!, request.Request.Country!);

        _metrics.IncrementActiveShelters();
    }
}
