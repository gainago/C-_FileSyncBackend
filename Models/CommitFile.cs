using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FileSyncBackend.Models;

public class CommitFile
{
    public Guid CommitId { get; set; }
    
    [JsonIgnore]  // ← ДОБАВЬ ЭТО
    public Commit Commit { get; set; } = null!;
    
    [MaxLength(1000)]
    public string Path { get; set; } = string.Empty;
    
    [MaxLength(64)]
    public string BlobHash { get; set; } = string.Empty;
    
    [JsonIgnore]  // ← ДОБАВЬ ЭТО
    public Blob Blob { get; set; } = null!;
}