using FileSyncBackend.Data;
using FileSyncBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FileSyncBackend.Repositories;

public class CommitRepository : ICommitRepository
{
    private readonly AppDbContext _context;
    
    public CommitRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Commit?> GetByIdAsync(Guid id)
    {
        return await _context.Commits
            .Include(c => c.Files)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<Commit?> GetLatestAsync()
    {
        return await _context.Commits
            .Include(c => c.Files)
            .OrderByDescending(c => c.Timestamp)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<Commit>> GetAllAsync()
    {
        return await _context.Commits
            .Include(c => c.Files)      //this row
            .OrderByDescending(c => c.Timestamp)
            .ToListAsync();
    }
    
    public async Task<Commit> CreateAsync(Commit commit)
    {
        _context.Commits.Add(commit);
        await _context.SaveChangesAsync();
        return commit;
    }
    
    public async Task<List<CommitFile>> GetFilesByCommitIdAsync(Guid commitId)
    {
        return await _context.CommitFiles
            .Where(cf => cf.CommitId == commitId)
            .ToListAsync();
    }
}