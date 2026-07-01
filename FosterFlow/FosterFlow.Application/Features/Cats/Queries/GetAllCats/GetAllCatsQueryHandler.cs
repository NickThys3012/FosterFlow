using FosterFlow.Contracts.DTOs.Cats.GetAllCats;
using FosterFlow.Domain.Enums;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Queries.GetAllCats;

public class GetAllCatsQueryHandler : IRequestHandler<GetAllCatsQuery, GetAllCatsResponse>
{

    private readonly ICatRepository _cats;
    public GetAllCatsQueryHandler(ICatRepository cats)
    {
        _cats = cats;
    }

    public async Task<GetAllCatsResponse> Handle(GetAllCatsQuery request, CancellationToken cancellationToken)
    {
        var cat = await _cats.GetAllFromShelterAsync(request.UserId, cancellationToken);

        var response = new GetAllCatsResponse
        {
            Cats = cat.Select(c => new GetAllCatsDto
            {
                CatName = c.Name,
                Id = c.Id,
                Age = c.Age,
                PhotoUrl = c.PhotoUrl,
                Sex = c.Sex,
                Status = c.Status,
                WeeksNeeded = c.FosterDuration,
                IsUrgent = c.IsUrgent,
                Matches = 0,                            // temporary 0 functionality isn't there
                FostersName = "Jef",                    //functionality isn't there
                ExperienceLevel = ExperienceLevel.None, //functionality isn't there
                HomeType = HomeType.Apartment,          //functionality isn't there
                HasDogs = true                          //functionality isn't there
            }).ToList()
        };

        return response;
    }
}
