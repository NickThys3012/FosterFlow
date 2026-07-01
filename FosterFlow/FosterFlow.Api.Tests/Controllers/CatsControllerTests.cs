using FosterFlow.Api.Controllers;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Application.Features.Cats.Commands.CreateCat;
using FosterFlow.Contracts.DTOs.Cats;
using FosterFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
namespace FosterFlow.Api.Tests.Controllers;

[TestFixture]
public class CatsControllerTests
{
    [Test]
    public async Task Create_ReturnsCreatedAtRouteWithCatId()
    {
        var mediator = Substitute.For<ISender>();
        var currentUserService = Substitute.For<ICurrentUserService>();
        var controller = new CatsController(mediator, currentUserService);
        var request = new CreateCatRequest
        {
            Name = "Mochi",
            DogFriendly = true,
            IsUrgent = false,
            Sex = Sex.Female,
            FosterDuration = 4,
            Age = 12,
            MedicalNeeds = string.Empty,
            PhotoUrl = string.Empty,
            TemperamentTags = ["Calm"]
        };
        var catId = Guid.NewGuid();

        currentUserService.UserId.Returns(Guid.NewGuid());
        mediator.Send(Arg.Any<CreateCatCommand>(), Arg.Any<CancellationToken>()).Returns(catId);

        var result = await controller.Create(request, CancellationToken.None) as CreatedAtRouteResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.RouteName, Is.EqualTo("GetCatById"));
        Assert.That(result.Value, Is.InstanceOf<CreateCatResponse>());
        Assert.That(((CreateCatResponse)result.Value!).Id, Is.EqualTo(catId));
    }
}
