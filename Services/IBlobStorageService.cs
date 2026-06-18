namespace FileSyncBackend.Services;

public interface IBlobStorageService
{
    Task<string> SaveBlobAsync(Stream fileStream, string? contentType = null);
    Task<Stream?> LoadBlobAsync(string hash);
    Task<bool> BlobExistsAsync(string hash);
    Task DeleteBlobAsync(string hash);
}