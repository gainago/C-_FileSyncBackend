using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FileSyncBackend.Models;
using FileSyncBackend.Repositories;
using Microsoft.Extensions.Configuration;

namespace FileSyncBackend.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly IBlobRepository _blobRepository;
    private readonly string _blobStoragePath;
    
    public BlobStorageService(IBlobRepository blobRepository, IConfiguration configuration)
    {
        _blobRepository = blobRepository;
        _blobStoragePath = configuration["BlobStorage:Path"] ?? "/data/blobs";
        
        // Создаём директорию, если её нет
        if (!Directory.Exists(_blobStoragePath))
        {
            Directory.CreateDirectory(_blobStoragePath);
        }
    }
    
    public async Task<string> SaveBlobAsync(Stream fileStream, string? contentType = null)
    {
        // Вычисляем хеш содержимого
        using var sha256 = SHA256.Create();
        var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        
        var hashBytes = await sha256.ComputeHashAsync(memoryStream);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        
        // Проверяем, существует ли уже такой blob
        if (await _blobRepository.ExistsAsync(hash))
        {
            return hash; // Дедупликация
        }
        
        // Сохраняем файл в файловую систему
        var filePath = Path.Combine(_blobStoragePath, hash);
        memoryStream.Position = 0;
        
        // ИСПРАВЛЕНИЕ: переименовали fileStream в outputStream, чтобы избежать конфликта с параметром
        using (var outputStream = new FileStream(filePath, FileMode.Create))
        {
            await memoryStream.CopyToAsync(outputStream);
        }
        
        // Создаём запись в БД
        var blob = new Blob
        {
            Hash = hash,
            Size = memoryStream.Length,
            ContentType = contentType,
            CreatedAt = DateTime.UtcNow
        };
        
        await _blobRepository.CreateAsync(blob);
        
        return hash;
    }
    
    public async Task<Stream?> LoadBlobAsync(string hash)
    {
        var filePath = Path.Combine(_blobStoragePath, hash);
        
        if (!File.Exists(filePath))
        {
            return null;
        }
        
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return fileStream;
    }
    
    public async Task<bool> BlobExistsAsync(string hash)
    {
        return await _blobRepository.ExistsAsync(hash);
    }
    
    // ИСПРАВЛЕНИЕ: метод не возвращает значение, поэтому убран Task и async
    public Task DeleteBlobAsync(string hash)
    {
        var filePath = Path.Combine(_blobStoragePath, hash);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        
        return Task.CompletedTask;
    }
}