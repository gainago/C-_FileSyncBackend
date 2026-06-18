using FileSyncBackend.Models;
using FileSyncBackend.Repositories;

namespace FileSyncBackend.Services;

public class CommitService : ICommitService
{
    private readonly ICommitRepository _commitRepository;
    
    public CommitService(ICommitRepository commitRepository)
    {
        _commitRepository = commitRepository;
    }
    
    public async Task<Commit?> GetLatestCommitAsync()
    {
        return await _commitRepository.GetLatestAsync();
    }
    
    public async Task<Commit?> GetCommitByIdAsync(Guid id)
    {
        return await _commitRepository.GetByIdAsync(id);
    }
    
    public async Task<List<Commit>> GetAllCommitsAsync()
    {
        return await _commitRepository.GetAllAsync();
    }
    
    public async Task<Commit> CreateCommitAsync(string author, string? comment, Dictionary<string, string> files, Guid? parentId = null)
    {
        var commit = new Commit
        {
            Author = author,
            Timestamp = DateTime.UtcNow,
            Comment = comment,
            ParentId = parentId
        };
        
        // Добавляем файлы
        foreach (var (path, blobHash) in files)
        {
            commit.Files.Add(new CommitFile
            {
                CommitId = commit.Id,
                Path = path,
                BlobHash = blobHash
            });
        }
        
        return await _commitRepository.CreateAsync(commit);
    }
    
    public async Task<Dictionary<string, string>> GetCommitFilesAsync(Guid commitId)
    {
        var files = await _commitRepository.GetFilesByCommitIdAsync(commitId);
        return files.ToDictionary(f => f.Path, f => f.BlobHash);
    }
}