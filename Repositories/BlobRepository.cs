using FileSyncBackend.Data;
using FileSyncBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FileSyncBackend.Repositories;

public class BlobRepository : IBlobRepository
{
    private readonly AppDbContext _context;
    
    public BlobRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Blob?> GetByHashAsync(string hash)
    {
        return await _context.Blobs.FirstOrDefaultAsync(b => b.Hash == hash);
    }
    
    public async Task<Blob> CreateAsync(Blob blob)
    {
        _context.Blobs.Add(blob);
        await _context.SaveChangesAsync();
        return blob;
    }
    
    public async Task<bool> ExistsAsync(string hash)
    {
        return await _context.Blobs.AnyAsync(b => b.Hash == hash);
    }
}