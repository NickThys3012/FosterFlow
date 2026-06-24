using FosterFlow.Domain.Enums;
namespace FosterFlow.Application.Common.Interfaces;

public interface IIdentityService
{
    Task RegisterAsync(string email, string password, string displayName, UserRole role);
}
