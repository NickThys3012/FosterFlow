using FosterFlow.Domain.Entities;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.CreateCat;

public class CreateCatCommandHandler : IRequestHandler<CreateCatCommand, Guid>
{
    private readonly ICatRepository _cats;

    public CreateCatCommandHandler(ICatRepository cats)
    {
        _cats = cats;
    }

    public async Task<Guid> Handle(CreateCatCommand cmd, CancellationToken cancellationToken)
    {
        var cat = new Cat(cmd.Request.Name, cmd.Request.BirthDate);
        await _cats.AddAsync(cat, cancellationToken);
        return cat.Id;
    }
}
