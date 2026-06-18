using FileSyncBackend.Models;

namespace FileSyncBackend.Repositories;

public interface IConflictRepository
{
    Task<Conflict> CreateAsync(Conflict conflict);
    Task<Conflict?> GetByIdAsync(Guid id);
    Task<List<Conflict>> GetUnresolvedAsync();
    Task UpdateAsync(Conflict conflict);
}