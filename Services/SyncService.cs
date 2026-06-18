using FileSyncBackend.Repositories;

namespace FileSyncBackend.Services;

public class SyncService : ISyncService
{
    private readonly ICommitService _commitService;
    private readonly ISyncSnapshotRepository _syncSnapshotRepository;
    private readonly IBlobStorageService _blobStorageService;
    
    public SyncService(
        ICommitService commitService,
        ISyncSnapshotRepository syncSnapshotRepository,
        IBlobStorageService blobStorageService)
    {
        _commitService = commitService;
        _syncSnapshotRepository = syncSnapshotRepository;
        _blobStorageService = blobStorageService;
    }
    
    public async Task<ServerStateDto> GetServerStateAsync()
    {
        var latestCommit = await _commitService.GetLatestCommitAsync();
        
        if (latestCommit == null)
        {
            return new ServerStateDto
            {
                CommitId = Guid.Empty,
                Timestamp = DateTime.MinValue,
                Files = new Dictionary<string, string>()
            };
        }
        
        var files = await _commitService.GetCommitFilesAsync(latestCommit.Id);
        
        return new ServerStateDto
        {
            CommitId = latestCommit.Id,
            Timestamp = latestCommit.Timestamp,
            Files = files
        };
    }
    
    public async Task<SyncResultDto> SyncAsync(string userId, Dictionary<string, string> clientState)
    {
        // Получаем текущее состояние сервера
        var serverState = await GetServerStateAsync();
        
        // Получаем снимок последней синхронизации для этого пользователя
        var syncSnapshot = await _syncSnapshotRepository.GetByUserIdAsync(userId);
        var baseState = syncSnapshot != null 
            ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(syncSnapshot.SnapshotData) 
            : new Dictionary<string, string>();
        
        // Сравниваем состояния и обнаруживаем конфликты
        var allPaths = clientState.Keys.Union(serverState.Files.Keys).ToHashSet();
        var conflicts = new List<string>();
        var newState = new Dictionary<string, string>();
        
        foreach (var path in allPaths)
        {
            var baseHash = baseState?.GetValueOrDefault(path);
            var clientHash = clientState.GetValueOrDefault(path);
            var serverHash = serverState.Files.GetValueOrDefault(path);
            
            // Простая логика: если есть конфликт, добавляем в список
            if (clientHash != null && serverHash != null && clientHash != serverHash)
            {
                if (baseHash == null || (clientHash != baseHash && serverHash != baseHash))
                {
                    conflicts.Add(path);
                    // Пока используем серверную версию
                    newState[path] = serverHash;
                    continue;
                }
            }
            
            // Если клиент изменил, а сервер нет — принимаем клиентскую версию
            if (clientHash != null && (baseHash == null || clientHash != baseHash) && serverHash == baseHash)
            {
                newState[path] = clientHash;
            }
            // Если сервер изменил, а клиент нет — принимаем серверную версию
            else if (serverHash != null)
            {
                newState[path] = serverHash;
            }
        }
        
        // Создаём новый коммит
        var newCommit = await _commitService.CreateCommitAsync(
            userId,
            $"Sync from client at {DateTime.UtcNow}",
            newState,
            serverState.CommitId == Guid.Empty ? null : serverState.CommitId
        );
        
        // Обновляем снимок синхронизации
        var snapshotJson = System.Text.Json.JsonSerializer.Serialize(newState);
        await _syncSnapshotRepository.CreateOrUpdateAsync(new Models.SyncSnapshot
        {
            UserId = userId,
            SnapshotData = snapshotJson,
            ServerCommitId = newCommit.Id,
            CreatedAt = DateTime.UtcNow
        });
        
        return new SyncResultDto
        {
            Success = true,
            Message = conflicts.Any() 
                ? $"Sync completed with {conflicts.Count} conflicts: {string.Join(", ", conflicts)}" 
                : "Sync completed successfully",
            NewCommitId = newCommit.Id
        };
    }
}