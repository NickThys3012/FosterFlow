using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Contracts.DTOs.Cats;
using FosterFlow.Domain.Entities;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Queries.GetCats;

public class GetCatQueryHandler : IRequestHandler<GetCatQuery, CatDto>
{

    private readonly ICatRepository _cats;
    public GetCatQueryHandler(ICatRepository cats)
    {
        _cats = cats;
    }

    public async Task<CatDto> Handle(GetCatQuery request, CancellationToken cancellationToken)
    {
        var cat = await _cats.GetByIdAsync(request.Id, cancellationToken);
        if (cat is null)
        {
            throw new NotFoundException(nameof(Cat), request.Id);
        }

        return new CatDto
        {
            Name = cat.Name, Status = cat.Status, Id = cat.Id
        };
    }
}
