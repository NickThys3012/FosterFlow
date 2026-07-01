using FosterFlow.Contracts.DTOs.Cats;
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
            throw new KeyNotFoundException($"Cat with ID {request.CatId} not found.");
        }

        var shelter = await _users.GetByIdAsync(Guid.Parse(cat.ShelterId));
        if (shelter == null)
        {
            throw new KeyNotFoundException($"Shelter with ID {cat.ShelterId} not found.");
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
            TempramentTags = cat.TemperamentTags,
            MedicalNeeds = cat.MedicalNeeds,
            ShelterName = shelter.Name,
            ShelterLocation = shelter.City
        };
    }
}
