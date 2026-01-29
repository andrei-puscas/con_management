using Backend.DTOs.Proiect;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProiecteController : ControllerBase
{
    private readonly IProiecteService _service;

    public ProiecteController(IProiecteService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProiectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? stare, CancellationToken cancellationToken)
    {
        var list = await _service.GetAllAsync(stare, cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProiectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    [ProducesResponseType(typeof(ProiectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProiectRequest request, CancellationToken cancellationToken)
    {
        if (request == null) return BadRequest();
        try
        {
            var item = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    [ProducesResponseType(typeof(ProiectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProiectRequest request, CancellationToken cancellationToken)
    {
        if (request == null) return BadRequest();
        try
        {
            var item = await _service.UpdateAsync(id, request, cancellationToken);
            if (item == null) return NotFound();
            return Ok(item);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
