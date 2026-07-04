using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Domain.Entities;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.CreateCat;

public class CreateCatCommandHandler : IRequestHandler<CreateCatCommand, Guid>
{
    private readonly ICatRepository _cats;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessMetrics _metrics;

    public CreateCatCommandHandler(ICatRepository cats, IBusinessMetrics metrics, ICurrentUserService currentUserService)
    {
        _cats = cats;
        _metrics = metrics;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateCatCommand cmd, CancellationToken cancellationToken)
    {
        var cat = new Cat(cmd.Request.Name, cmd.Request.DogFriendly, cmd.Request.IsUrgent, cmd.Request.Sex, _currentUserService.UserId, cmd.Request.Age, cmd.Request.TemperamentTags,
            cmd.Request.PhotoUrl,
            cmd.Request.FosterDuration, cmd.Request.MedicalNeeds)
        {
            CreatedDate = DateTime.UtcNow
        };
        await _cats.AddAsync(cat, cancellationToken);
        _metrics.CatListingCreated();
        return cat.Id;
    }
}
