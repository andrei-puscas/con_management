using Backend.DTOs.ProiectComentariu;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/proiecte/{proiectId:int}/comentarii")]
[Authorize]
public class ProiectComentariiController : ControllerBase
{
    private readonly IProiectComentariiService _service;

    public ProiectComentariiController(IProiectComentariiService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProiectComentariuDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProiectId(int proiectId, CancellationToken cancellationToken)
    {
        var list = await _service.GetByProiectIdAsync(proiectId, cancellationToken);
        return Ok(list);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProiectComentariuDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(int proiectId, [FromBody] CreateProiectComentariuRequest request, CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Text)) return BadRequest("Text obligatoriu.");
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        var result = await _service.CreateAsync(proiectId, userId, request, cancellationToken);
        if (result == null) return NotFound("Proiect negăsit.");
        return CreatedAtAction(nameof(GetByProiectId), new { proiectId }, result);
    }

    [HttpDelete("{comentariuId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int proiectId, int comentariuId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = userIdClaim != null && int.TryParse(userIdClaim, out var id) ? id : (int?)null;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var deleted = await _service.DeleteAsync(proiectId, comentariuId, userId, userRole, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
