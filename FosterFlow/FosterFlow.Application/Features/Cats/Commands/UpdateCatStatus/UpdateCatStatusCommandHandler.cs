using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Domain.Enums;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.UpdateCatStatus;

public class UpdateCatStatusCommandHandler : IRequestHandler<UpdateCatStatusCommand>
{
    private readonly ICatRepository _cats;
    private readonly ICurrentUserService _currentUserService;
    public UpdateCatStatusCommandHandler(ICatRepository cats, ICurrentUserService currentUserService)
    {
        _cats = cats;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateCatStatusCommand request, CancellationToken cancellationToken)
    {
        var cat = await _cats.GetByIdAsync(request.CatId, cancellationToken);
        if (cat == null)
        {
            throw new NotFoundException(nameof(cat), request.CatId);
        }

        if (cat.ShelterId != _currentUserService.UserId.ToString())
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user is null || user.Role != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("User is not authorized to update this cat's status.");
            }
        }

        cat.Status = request.Request.CatStatus;
        cat.IsArchived = request.Request.CatStatus is CatStatus.Adopted or CatStatus.Returned;

        await _cats.UpdateAsync(cat, cancellationToken);
    }
}
