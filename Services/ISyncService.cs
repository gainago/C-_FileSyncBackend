namespace FileSyncBackend.Services;

public interface ISyncService
{
    Task<ServerStateDto> GetServerStateAsync();
    Task<SyncResultDto> SyncAsync(string userId, Dictionary<string, string> clientState);
}

public class ServerStateDto
{
    public Guid CommitId { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> Files { get; set; } = new();
}

public class SyncResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? NewCommitId { get; set; }
}