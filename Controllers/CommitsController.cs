using Microsoft.AspNetCore.Mvc;
using FileSyncBackend.Services;

namespace FileSyncBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommitsController : ControllerBase
{
    private readonly ICommitService _commitService;
    
    public CommitsController(ICommitService commitService)
    {
        _commitService = commitService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllCommits()
    {
        var commits = await _commitService.GetAllCommitsAsync();
        return Ok(commits);
    }
    
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestCommit()
    {
        var commit = await _commitService.GetLatestCommitAsync();
        
        if (commit == null)
        {
            return NotFound();
        }
        
        return Ok(commit);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommitById(Guid id)
    {
        var commit = await _commitService.GetCommitByIdAsync(id);
        
        if (commit == null)
        {
            return NotFound();
        }
        
        return Ok(commit);
    }
    
    [HttpGet("{id}/files")]
    public async Task<IActionResult> GetCommitFiles(Guid id)
    {
        var files = await _commitService.GetCommitFilesAsync(id);
        return Ok(files);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCommit([FromBody] CreateCommitRequest request)
    {
        var commit = await _commitService.CreateCommitAsync(
            request.Author,
            request.Comment,
            request.Files,
            request.ParentId
        );
        
        return Ok(commit);
    }
}

public class CreateCommitRequest
{
    public string Author { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public Dictionary<string, string> Files { get; set; } = new();
    public Guid? ParentId { get; set; }
}