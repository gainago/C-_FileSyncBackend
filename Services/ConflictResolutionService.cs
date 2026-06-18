using FileSyncBackend.Models;
using FileSyncBackend.Repositories;

namespace FileSyncBackend.Services;

public class ConflictResolutionService : IConflictResolutionService
{
    private readonly IConflictRepository _conflictRepository;
    
    public ConflictResolutionService(IConflictRepository conflictRepository)
    {
        _conflictRepository = conflictRepository;
    }
    
    public async Task<Conflict> CreateConflictAsync(string path, string? baseHash, string localHash, string serverHash)
    {
        var conflict = new Conflict
        {
            Path = path,
            BaseBlobHash = baseHash,
            LocalBlobHash = localHash,
            ServerBlobHash = serverHash,
            CreatedAt = DateTime.UtcNow
        };
        
        return await _conflictRepository.CreateAsync(conflict);
    }
    
    public async Task<List<Conflict>> GetUnresolvedConflictsAsync()
    {
        return await _conflictRepository.GetUnresolvedAsync();
    }
    
    public async Task ResolveConflictAsync(Guid conflictId, string resolvedBlobHash, string resolvedBy)
    {
        var conflict = await _conflictRepository.GetByIdAsync(conflictId);
        
        if (conflict == null)
        {
            throw new Exception($"Conflict {conflictId} not found");
        }
        
        conflict.ResolvedBlobHash = resolvedBlobHash;
        conflict.ResolvedBy = resolvedBy;
        conflict.ResolvedAt = DateTime.UtcNow;
        
        await _conflictRepository.UpdateAsync(conflict);
    }
}