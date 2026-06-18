using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FileSyncBackend.Models;

public class SyncSnapshot
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [MaxLength(100)]
    public string UserId { get; set; } = string.Empty;
    
    // JSON-представление снимка (словарь "путь → хеш")
    [Column(TypeName = "jsonb")]
    public string SnapshotData { get; set; } = "{}";
    
    public Guid ServerCommitId { get; set; }
    public Commit ServerCommit { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}