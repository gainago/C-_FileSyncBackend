using System.ComponentModel.DataAnnotations;

namespace FileSyncBackend.Models;

public class Commit
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Author { get; set; } = string.Empty; // ID пользователя
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string? Comment { get; set; }
    
    public Guid? ParentId { get; set; } // Ссылка на родительский коммит
    
    // Навигационные свойства
    public Commit? Parent { get; set; }
    public List<CommitFile> Files { get; set; } = new();
}