using FosterFlow.Contracts.DTOs.Cats;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Queries.GetCat;

public class GetCatQueryHandler : IRequestHandler<GetCatQuery, CatDto>
{
    private readonly ICatRepository _cats;

    public GetCatQueryHandler(ICatRepository cats)
    {
        _cats = cats;
    }

    public async Task<CatDto> Handle(GetCatQuery request, CancellationToken cancellationToken)
    {
        var cat = await _cats.GetByIdAsync(request.CatId, cancellationToken);
        if (cat == null)
        {
            throw new KeyNotFoundException($"Cat with ID {request.CatId} not found.");
        }

        return new CatDto
        {
            Id = cat.Id,
            Name = cat.Name
        };
    }
}
