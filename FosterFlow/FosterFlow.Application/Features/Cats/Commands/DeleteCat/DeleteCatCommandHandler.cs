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
        if (cat.ShelterId != _currentUserService.UserId.ToString())
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user is null || user.Role != FosterFlow.Domain.Enums.UserRole.Admin)
            {
                throw new UnauthorizedAccessException("User is not authorized to delete this cat.");
            }
        }

        cat.IsDeleted = true;
        await _cats.UpdateAsync(cat, cancellationToken);
    }
}
