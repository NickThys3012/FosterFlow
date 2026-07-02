using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Domain.Entities;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.UpdateCat;

public class UpdateCatCommandHandler : IRequestHandler<UpdateCatCommand>
{
    private readonly ICatRepository _cats;
    public UpdateCatCommandHandler(ICatRepository cats)
    {
        _cats = cats;
    }

    public async Task Handle(UpdateCatCommand request, CancellationToken cancellationToken)
    {
        var cat = await _cats.GetByIdAsync(request.Request.Id, cancellationToken);
        if (cat == null)
        {
            throw new NotFoundException(nameof(Cat), request.Request.Id);
        }

        cat.Name = request.Request.Name;
        cat.Age = request.Request.Age;
        cat.PhotoUrl = request.Request.PhotoUrl;
        cat.TemperamentTags = request.Request.TemperamentTags;
        cat.MedicalNeeds = request.Request.MedicalNeeds;
        cat.FosterDuration = request.Request.FosterDuration;
        cat.IsUrgent = request.Request.IsUrgent;
        cat.DogFriendly = request.Request.DogFriendly;
        cat.Sex = request.Request.Sex;
        await _cats.UpdateAsync(cat, cancellationToken);
    }
}
