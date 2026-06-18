using FileSyncBackend.Models;

namespace FileSyncBackend.Services;

public interface IConflictResolutionService
{
    Task<Conflict> CreateConflictAsync(string path, string? baseHash, string localHash, string serverHash);
    Task<List<Conflict>> GetUnresolvedConflictsAsync();
    Task ResolveConflictAsync(Guid conflictId, string resolvedBlobHash, string resolvedBy);
}