using Backend.DTOs.Deviz;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/devize")]
[Authorize]
public class DevizeGlobalController : ControllerBase
{
    private readonly IDevizeService _service;

    public DevizeGlobalController(IDevizeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var devize = await _service.GetAllAsync();
        return Ok(devize);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var deviz = await _service.GetByIdAsync(id);
        if (deviz == null) return NotFound();
        return Ok(deviz);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateDevizGlobalRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var req = new CreateDevizRequest
        {
            Titlu = request.Titlu,
            NumarInregistrare = request.NumarInregistrare,
            Beneficiar = request.Beneficiar,
            Executant = request.Executant,
            CotaTVA = request.CotaTVA,
            Data = request.Data,
            Linii = request.Linii
        };
        var deviz = await _service.CreateAsync(request.ProiectId, req);
        return CreatedAtAction(nameof(GetById), new { id = deviz.Id }, deviz);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDevizRequest request)
    {
        var deviz = await _service.UpdateAsync(id, request);
        if (deviz == null) return NotFound();
        return Ok(deviz);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdf(int id)
    {
        var deviz = await _service.GetByIdAsync(id);
        if (deviz == null) return NotFound();

        var pdf = await _service.GeneratePdfAsync(id);
        var fileName = $"deviz_{deviz.Titlu.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf";
        return File(pdf, "application/pdf", fileName);
    }
}
