using System.ComponentModel.DataAnnotations;

namespace FileSyncBackend.Models;

public class CommitFile
{
    public Guid CommitId { get; set; }
    public Commit Commit { get; set; } = null!;
    
    [MaxLength(1000)]
    public string Path { get; set; } = string.Empty; // Путь+имя файла
    
    [MaxLength(64)]
    public string BlobHash { get; set; } = string.Empty; // Хеш blob-объекта
    public Blob Blob { get; set; } = null!;
}