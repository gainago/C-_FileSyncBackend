using FileSyncBackend.Models;

namespace FileSyncBackend.Repositories;

public interface ICommitRepository
{
    Task<Commit?> GetByIdAsync(Guid id);
    Task<Commit?> GetLatestAsync();
    Task<List<Commit>> GetAllAsync();
    Task<Commit> CreateAsync(Commit commit);
    Task<List<CommitFile>> GetFilesByCommitIdAsync(Guid commitId);
}