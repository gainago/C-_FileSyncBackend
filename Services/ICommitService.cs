using FileSyncBackend.Models;

namespace FileSyncBackend.Services;

public interface ICommitService
{
    Task<Commit?> GetLatestCommitAsync();
    Task<Commit?> GetCommitByIdAsync(Guid id);
    Task<List<Commit>> GetAllCommitsAsync();
    Task<Commit> CreateCommitAsync(string author, string? comment, Dictionary<string, string> files, Guid? parentId = null);
    Task<Dictionary<string, string>> GetCommitFilesAsync(Guid commitId);
}