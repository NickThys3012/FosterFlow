using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.UpdateCatStatus;

public class UpdateCatStatusCommandHandler : IRequestHandler<UpdateCatStatusCommand>
{
    private readonly ICatRepository _cats;
    public UpdateCatStatusCommandHandler(ICatRepository cats)
    {
        _cats = cats;
    }

    public async Task Handle(UpdateCatStatusCommand request, CancellationToken cancellationToken)
    {
        var cat = await _cats.GetByIdAsync(request.CatId, cancellationToken);
        if (cat == null)
        {
            throw new NotFoundException(nameof(cat), request.CatId);
        }

        if (cat.ShelterId != request.UserId.ToString())
        {
            throw new UnauthorizedAccessException("User is not authorized to update this cat's status.");
        }

        cat.Status = request.Request.CatStatus;
        await _cats.UpdateAsync(cat, cancellationToken);
    }
}
