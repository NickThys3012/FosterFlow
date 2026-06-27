using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Application.Features.Auth.Commands.RegisterShelter;
using FosterFlow.Contracts.DTOs.Auth;
using FosterFlow.Domain.Entities;
using FosterFlow.Domain.Enums;
using FosterFlow.Domain.Interfaces.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace FosterFlow.Application.Tests.Features.Auth;

[TestFixture]
public class RegisterShelterCommandHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _identityService = Substitute.For<IIdentityService>();
        _metrics = Substitute.For<IBusinessMetrics>();
        _handler = new RegisterShelterCommandHandler(_identityService, _userRepository, _metrics);
    }

    private IUserRepository _userRepository = null!;
    private IIdentityService _identityService = null!;
    private IBusinessMetrics _metrics = null!;
    private RegisterShelterCommandHandler _handler = null!;

    private static RegisterShelterCommand ValidCommand(string email = "shelter@example.com")
    {
        return new RegisterShelterCommand(new RegisterShelterRequest
        {
            Name = "Happy Paws Shelter",
            Email = email,
            Password = "Passw0rd!",
            Phone = "+3212345678",
            Street = "Main Street 1",
            PostalCode = "2000",
            City = "Antwerp",
            Country = "Belgium"
        });
    }

    private static User ExistingUser(string email)
    {
        return new User(Guid.NewGuid(), email, "Existing", UserRole.Shelter, "Street", "1000", "City", "Country", null);
    }

    [Test]
    public void Handle_WhenEmailAlreadyExists_ThrowsValidationException()
    {
        var command = ValidCommand();
        _userRepository.GetByEmailAsync(command.Request.Email).Returns(ExistingUser(command.Request.Email));

        Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_WhenEmailAlreadyExists_ReportsEmailError()
    {
        var command = ValidCommand();
        _userRepository.GetByEmailAsync(command.Request.Email).Returns(ExistingUser(command.Request.Email));

        var ex = Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.That(ex!.Errors, Does.ContainKey("Email"));
    }

    [Test]
    public async Task Handle_WhenEmailAlreadyExists_DoesNotCallIdentityService()
    {
        var command = ValidCommand();
        _userRepository.GetByEmailAsync(command.Request.Email).Returns(ExistingUser(command.Request.Email));

        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (ValidationException)
        {
            // expected
        }

        await _identityService.DidNotReceiveWithAnyArgs().RegisterShelterAsync(
            default!, default!, default!, default!, default!, default!, default!, default!);
    }

    [Test]
    public async Task Handle_WhenEmailAlreadyExists_DoesNotIncrementActiveShelters()
    {
        var command = ValidCommand();
        _userRepository.GetByEmailAsync(command.Request.Email).Returns(ExistingUser(command.Request.Email));

        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (ValidationException)
        {
            // expected
        }

        _metrics.DidNotReceive().IncrementActiveShelters();
    }

    [Test]
    public async Task Handle_WithNewEmail_LooksUpUserByRequestEmail()
    {
        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        await _userRepository.Received(1).GetByEmailAsync(command.Request.Email);
    }

    [Test]
    public async Task Handle_WithNewEmail_CallsRegisterShelterAsyncWithAllRequestValues()
    {
        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        await _identityService.Received(1).RegisterShelterAsync(
            command.Request.Email,
            command.Request.Password,
            command.Request.Name,
            command.Request.Phone!,
            command.Request.Street!,
            command.Request.PostalCode!,
            command.Request.City!,
            command.Request.Country!);
    }

    [Test]
    public async Task Handle_WithNewEmail_IncrementsActiveSheltersExactlyOnce()
    {
        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        _metrics.Received(1).IncrementActiveShelters();
    }

    [Test]
    public void Handle_WhenIdentityServiceThrows_PropagatesException()
    {
        var command = ValidCommand();
        _identityService.RegisterShelterAsync(
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new InvalidOperationException("identity failure"));

        Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_WhenIdentityServiceThrows_DoesNotIncrementActiveShelters()
    {
        var command = ValidCommand();
        _identityService.RegisterShelterAsync(
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new InvalidOperationException("identity failure"));

        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            // expected
        }

        _metrics.DidNotReceive().IncrementActiveShelters();
    }
}
