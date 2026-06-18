using FileSyncBackend.Data;
using FileSyncBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FileSyncBackend.Repositories;

public class ConflictRepository : IConflictRepository
{
    private readonly AppDbContext _context;
    
    public ConflictRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Conflict> CreateAsync(Conflict conflict)
    {
        _context.Conflicts.Add(conflict);
        await _context.SaveChangesAsync();
        return conflict;
    }
    
    public async Task<Conflict?> GetByIdAsync(Guid id)
    {
        return await _context.Conflicts.FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<List<Conflict>> GetUnresolvedAsync()
    {
        return await _context.Conflicts
            .Where(c => c.ResolvedBlobHash == null)
            .ToListAsync();
    }
    
    public async Task UpdateAsync(Conflict conflict)
    {
        _context.Conflicts.Update(conflict);
        await _context.SaveChangesAsync();
    }
}