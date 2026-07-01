using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Contracts.DTOs.Cats;
using FosterFlow.Domain.Entities;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Queries.GetCat;

public class GetCatQueryHandler : IRequestHandler<GetCatQuery, CatDto>
{
    private readonly ICatRepository _cats;
    private readonly IUserRepository _users;
    public GetCatQueryHandler(ICatRepository cats, IUserRepository users)
    {
        _cats = cats;
        _users = users;
    }

    public async Task<CatDto> Handle(GetCatQuery request, CancellationToken cancellationToken)
    {
        var cat = await _cats.GetByIdAsync(request.CatId, cancellationToken);
        if (cat == null)
        {
            throw new NotFoundException(nameof(Cat), request.CatId);
        }

        var shelter = await _users.GetByIdAsync(Guid.Parse(cat.ShelterId));
        if (shelter == null)
        {
            throw new NotFoundException("Shelter", cat.ShelterId);
        }
        return new CatDto
        {
            Id = cat.Id,
            CatName = cat.Name,
            CatAge = cat.Age,
            CatPhotoUrl = cat.PhotoUrl,
            CatSex = cat.Sex,
            CatStatus = cat.Status,
            FosterDuration = cat.FosterDuration,
            DogFriendly = cat.DogFriendly,
            IsUrgent = cat.IsUrgent,
            TemperamentTags = cat.TemperamentTags,
            MedicalNeeds = cat.MedicalNeeds,
            ShelterName = shelter.Name,
            ShelterLocation = shelter.City
        };
    }
}
