using Microsoft.AspNetCore.Mvc;
using FileSyncBackend.Services;

namespace FileSyncBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;
    
    public FilesController(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }
        
        using var stream = file.OpenReadStream();
        var hash = await _blobStorageService.SaveBlobAsync(stream, file.ContentType);
        
        return Ok(new
        {
            hash,
            fileName = file.FileName,
            size = file.Length,
            contentType = file.ContentType
        });
    }
    
    [HttpGet("{hash}")]
    public async Task<IActionResult> DownloadFile(string hash)
    {
        var stream = await _blobStorageService.LoadBlobAsync(hash);
        
        if (stream == null)
        {
            return NotFound();
        }
        
        return File(stream, "application/octet-stream", hash);
    }
    
    [HttpGet("{hash}/exists")]
    public async Task<IActionResult> CheckBlobExists(string hash)
    {
        var exists = await _blobStorageService.BlobExistsAsync(hash);
        return Ok(new { exists });
    }
}