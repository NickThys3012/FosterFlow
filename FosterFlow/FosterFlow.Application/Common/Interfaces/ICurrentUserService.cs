using FosterFlow.Domain.Entities;
namespace FosterFlow.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Task<User?> GetCurrentUserAsync();
}
