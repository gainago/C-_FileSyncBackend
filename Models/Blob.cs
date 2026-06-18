using System.ComponentModel.DataAnnotations;

namespace FileSyncBackend.Models;

public class Blob
{
    [Key]
    [MaxLength(64)]
    public string Hash { get; set; } = string.Empty; // SHA-256 хеш
    
    public long Size { get; set; } // Размер в байтах
    
    [MaxLength(100)]
    public string? ContentType { get; set; } // MIME-тип
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}