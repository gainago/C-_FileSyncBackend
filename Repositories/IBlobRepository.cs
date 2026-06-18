using FileSyncBackend.Models;

namespace FileSyncBackend.Repositories;

public interface IBlobRepository
{
    Task<Blob?> GetByHashAsync(string hash);
    Task<Blob> CreateAsync(Blob blob);
    Task<bool> ExistsAsync(string hash);
}