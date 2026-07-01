using FosterFlow.Domain.Entities;
namespace FosterFlow.Domain.Interfaces.Repositories;

public interface ICatRepository
{
    Task<Cat?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Cat>> GetAllAsync(CancellationToken ct = default);
    Task<List<Cat>> GetAllFromShelterAsync(Guid shelterId, CancellationToken ct = default);
    Task AddAsync(Cat cat, CancellationToken ct = default);
    Task UpdateAsync(Cat cat, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
