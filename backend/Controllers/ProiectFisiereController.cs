using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/proiecte/{proiectId:int}/fisiere")]
[Authorize]
public class ProiectFisiereController : ControllerBase
{
    private readonly IProiectFisiereService _service;

    public ProiectFisiereController(IProiectFisiereService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(int proiectId, CancellationToken ct)
    {
        var list = await _service.GetByProiectIdAsync(proiectId, ct);
        return Ok(list);
    }

    [HttpPost]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB maxim
    public async Task<IActionResult> Upload(int proiectId, IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0) return BadRequest("Fișier invalid.");
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId)) return Unauthorized();
        var result = await _service.UploadAsync(proiectId, userId, file, ct);
        if (result == null) return NotFound("Proiect negăsit.");
        return Ok(result);
    }

    [HttpGet("{fisierId:int}/download")]
    public async Task<IActionResult> Download(int proiectId, int fisierId, CancellationToken ct)
    {
        var result = await _service.GetFileForDownloadAsync(proiectId, fisierId, ct);
        if (result == null) return NotFound("Fișier negăsit.");
        var (continut, numeOriginal, tipFisier) = result.Value;
        return File(continut, tipFisier, numeOriginal);
    }

    [HttpDelete("{fisierId:int}")]
    public async Task<IActionResult> Delete(int proiectId, int fisierId, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = userIdClaim != null && int.TryParse(userIdClaim, out var id) ? id : (int?)null;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var deleted = await _service.DeleteAsync(proiectId, fisierId, userId, userRole, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
