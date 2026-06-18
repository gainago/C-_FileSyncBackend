using FileSyncBackend.Data;
using FileSyncBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FileSyncBackend.Repositories;

public class SyncSnapshotRepository : ISyncSnapshotRepository
{
    private readonly AppDbContext _context;
    
    public SyncSnapshotRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<SyncSnapshot?> GetByUserIdAsync(string userId)
    {
        return await _context.SyncSnapshots
            .Include(s => s.ServerCommit)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }
    
    public async Task<SyncSnapshot> CreateOrUpdateAsync(SyncSnapshot snapshot)
    {
        var existing = await _context.SyncSnapshots
            .FirstOrDefaultAsync(s => s.UserId == snapshot.UserId);
        
        if (existing != null)
        {
            existing.SnapshotData = snapshot.SnapshotData;
            existing.ServerCommitId = snapshot.ServerCommitId;
            existing.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            _context.SyncSnapshots.Add(snapshot);
        }
        
        await _context.SaveChangesAsync();
        return existing ?? snapshot;
    }
}