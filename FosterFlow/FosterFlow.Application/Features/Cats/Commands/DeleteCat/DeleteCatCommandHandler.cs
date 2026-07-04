using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.DeleteCat;

public class DeleteCatCommandHandler : IRequestHandler<DeleteCatCommand>
{
    private readonly ICatRepository _cats;
    private readonly ICurrentUserService _currentUserService;
    public DeleteCatCommandHandler(ICatRepository cats, ICurrentUserService currentUserService)
    {
        _cats = cats;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteCatCommand request, CancellationToken cancellationToken)
    {
        var cat = await _cats.GetByIdAsync(request.CatId, cancellationToken);
        if (cat == null)
        {
            throw new NotFoundException(nameof(cat), request.CatId);
        }
        if (Guid.Parse(cat.ShelterId) != _currentUserService.UserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this cat");
        }
        await _cats.DeleteAsync(request.CatId, cancellationToken);
    }
}
