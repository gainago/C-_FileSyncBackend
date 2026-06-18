using Microsoft.AspNetCore.Mvc;
using FileSyncBackend.Services;

namespace FileSyncBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConflictsController : ControllerBase
{
    private readonly IConflictResolutionService _conflictService;
    
    public ConflictsController(IConflictResolutionService conflictService)
    {
        _conflictService = conflictService;
    }
    
    [HttpGet("unresolved")]
    public async Task<IActionResult> GetUnresolvedConflicts()
    {
        var conflicts = await _conflictService.GetUnresolvedConflictsAsync();
        return Ok(conflicts);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateConflict([FromBody] CreateConflictRequest request)
    {
        var conflict = await _conflictService.CreateConflictAsync(
            request.Path,
            request.BaseBlobHash,
            request.LocalBlobHash,
            request.ServerBlobHash
        );
        
        return Ok(conflict);
    }
    
    [HttpPost("{id}/resolve")]
    public async Task<IActionResult> ResolveConflict(Guid id, [FromBody] ResolveConflictRequest request)
    {
        await _conflictService.ResolveConflictAsync(id, request.ResolvedBlobHash, request.ResolvedBy);
        return Ok(new { message = "Conflict resolved" });
    }
}

public class CreateConflictRequest
{
    public string Path { get; set; } = string.Empty;
    public string? BaseBlobHash { get; set; }
    public string LocalBlobHash { get; set; } = string.Empty;
    public string ServerBlobHash { get; set; } = string.Empty;
}

public class ResolveConflictRequest
{
    public string ResolvedBlobHash { get; set; } = string.Empty;
    public string ResolvedBy { get; set; } = string.Empty;
}