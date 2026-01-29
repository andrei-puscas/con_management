using Backend.DTOs.Angajat;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AngajatiController : ControllerBase
{
    private readonly IAngajatiService _service;

    public AngajatiController(IAngajatiService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AngajatDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? echipaId, CancellationToken cancellationToken)
    {
        var list = await _service.GetAllAsync(echipaId, cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AngajatDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    [ProducesResponseType(typeof(AngajatDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAngajatRequest request, CancellationToken cancellationToken)
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
    [ProducesResponseType(typeof(AngajatDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAngajatRequest request, CancellationToken cancellationToken)
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
