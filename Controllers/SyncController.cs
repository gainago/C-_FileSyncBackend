using Microsoft.AspNetCore.Mvc;
using FileSyncBackend.Services;

namespace FileSyncBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;
    
    public SyncController(ISyncService syncService)
    {
        _syncService = syncService;
    }
    
    [HttpGet("state")]
    public async Task<IActionResult> GetServerState()
    {
        var state = await _syncService.GetServerStateAsync();
        return Ok(state);
    }
    
    [HttpPost("sync")]
    public async Task<IActionResult> Sync([FromBody] SyncRequest request)
    {
        var result = await _syncService.SyncAsync(request.UserId, request.ClientState);
        return Ok(result);
    }
}

public class SyncRequest
{
    public string UserId { get; set; } = string.Empty;
    public Dictionary<string, string> ClientState { get; set; } = new();
}