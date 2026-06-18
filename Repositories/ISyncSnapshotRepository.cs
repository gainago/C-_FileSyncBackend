using FileSyncBackend.Models;

namespace FileSyncBackend.Repositories;

public interface ISyncSnapshotRepository
{
    Task<SyncSnapshot?> GetByUserIdAsync(string userId);
    Task<SyncSnapshot> CreateOrUpdateAsync(SyncSnapshot snapshot);
}