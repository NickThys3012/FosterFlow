using FosterFlow.Domain.Entities;
using FosterFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
namespace FosterFlow.Infrastructure.Persistence.Repositories;

public class CatRepository : ICatRepository
{
    private readonly AppDbContext _context;

    public CatRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Cat?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _context.Cats.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
    public Task<List<Cat>> GetAllAsync(CancellationToken ct = default)
    {
        return _context.Cats.ToListAsync(ct);
    }
    public Task AddAsync(Cat cat, CancellationToken ct = default)
    {
        _context.Cats.Add(cat);
        return _context.SaveChangesAsync(ct);
    }
    public Task UpdateAsync(Cat cat, CancellationToken ct = default)
    {
        _context.Cats.Update(cat);
        return _context.SaveChangesAsync(ct);
    }
    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var cat = await _context.Cats.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (cat != null)
        {
            _context.Cats.Remove(cat);
        }
        await _context.SaveChangesAsync(ct);
    }
}
