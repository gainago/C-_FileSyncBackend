using System.ComponentModel.DataAnnotations;

namespace FileSyncBackend.Models;

public class Conflict
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [MaxLength(1000)]
    public string Path { get; set; } = string.Empty;
    
    [MaxLength(64)]
    public string? BaseBlobHash { get; set; } // Может быть null
    
    [MaxLength(64)]
    public string LocalBlobHash { get; set; } = string.Empty;
    
    [MaxLength(64)]
    public string ServerBlobHash { get; set; } = string.Empty;
    
    [MaxLength(64)]
    public string? ResolvedBlobHash { get; set; }
    
    public string? ResolvedBy { get; set; }
    
    public DateTime? ResolvedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}