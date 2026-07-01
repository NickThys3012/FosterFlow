using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Domain.Entities;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.CreateCat;

public class CreateCatCommandHandler : IRequestHandler<CreateCatCommand, Guid>
{
    private readonly ICatRepository _cats;
    private readonly IBusinessMetrics _metrics;

    public CreateCatCommandHandler(ICatRepository cats, IBusinessMetrics metrics)
    {
        _cats = cats;
        _metrics = metrics;
    }

    public async Task<Guid> Handle(CreateCatCommand cmd, CancellationToken cancellationToken)
    {
        var cat = new Cat(cmd.Request.Name, cmd.Request.DogFriendly, cmd.Request.IsUrgent, cmd.Request.Sex, cmd.ShelterId, cmd.Request.Age, cmd.Request.TemperamentTags, cmd.Request.PhotoUrl,
            cmd.Request.FosterDuration, cmd.Request.MedicalNeeds)
        {
            CreateDate = DateTime.UtcNow
        };
        await _cats.AddAsync(cat, cancellationToken);
        _metrics.CatListingCreated();
        return cat.Id;
    }
}
