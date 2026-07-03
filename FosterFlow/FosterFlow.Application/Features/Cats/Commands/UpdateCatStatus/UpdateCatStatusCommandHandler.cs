using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace FosterFlow.Application.Features.Cats.Commands.UpdateCatStatus;

public class UpdateCatStatusCommandHandler : IRequestHandler<UpdateCatStatusCommand>
{
    private readonly ICatRepository _cats;
    private readonly IUserRepository _users;
    public UpdateCatStatusCommandHandler(ICatRepository cats, IUserRepository users)
    {
        _cats = cats;
        _users = users;
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
            var user = await _users.GetByIdAsync(request.UserId);
            if (user is null || user.Role != FosterFlow.Domain.Enums.UserRole.Admin)
            {
                throw new UnauthorizedAccessException("User is not authorized to update this cat's status.");
            }
        }

        cat.Status = request.Request.CatStatus;
        cat.IsArchived = request.Request.CatStatus is FosterFlow.Domain.Enums.CatStatus.Adopted or FosterFlow.Domain.Enums.CatStatus.Returned;

        await _cats.UpdateAsync(cat, cancellationToken);
    }
}
