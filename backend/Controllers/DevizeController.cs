using Backend.DTOs.Deviz;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/proiecte/{proiectId}/devize")]
[Authorize]
public class DevizeController : ControllerBase
{
    private readonly IDevizeService _service;

    public DevizeController(IDevizeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int proiectId)
    {
        var devize = await _service.GetByProiectIdAsync(proiectId);
        return Ok(devize);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int proiectId, int id)
    {
        var deviz = await _service.GetByIdAsync(id);
        if (deviz == null || deviz.ProiectId != proiectId) return NotFound();
        return Ok(deviz);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Create(int proiectId, [FromBody] CreateDevizRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var deviz = await _service.CreateAsync(proiectId, request);
        return CreatedAtAction(nameof(GetById), new { proiectId, id = deviz.Id }, deviz);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Update(int proiectId, int id, [FromBody] UpdateDevizRequest request)
    {
        var deviz = await _service.UpdateAsync(id, request);
        if (deviz == null || deviz.ProiectId != proiectId) return NotFound();
        return Ok(deviz);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Delete(int proiectId, int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdf(int proiectId, int id)
    {
        var deviz = await _service.GetByIdAsync(id);
        if (deviz == null || deviz.ProiectId != proiectId) return NotFound();

        var pdf = await _service.GeneratePdfAsync(id);
        var fileName = $"deviz_{deviz.Titlu.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf";
        return File(pdf, "application/pdf", fileName);
    }
}
