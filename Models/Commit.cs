using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FileSyncBackend.Models;

public class Commit
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Author { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string? Comment { get; set; }
    
    public Guid? ParentId { get; set; }
    
    // Навигационные свойства
    [JsonIgnore]  // ← ДОБАВЬ ЭТО
    public Commit? Parent { get; set; }
    
    public List<CommitFile> Files { get; set; } = new();
}