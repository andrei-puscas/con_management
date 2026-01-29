using Backend.Data;
using Backend.DTOs.Proiect;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class ProiecteService : IProiecteService
{
    private readonly AppDbContext _db;

    public ProiecteService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ProiectDto>> GetAllAsync(string? stare = null, CancellationToken cancellationToken = default)
    {
        var q = _db.Proiecte.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(stare))
            q = q.Where(p => p.Stare == stare);
        return await q.OrderBy(p => p.Nume)
            .Select(p => new ProiectDto { Id = p.Id, Nume = p.Nume, Client = p.Client, DataStart = p.DataStart, DataSfarsit = p.DataSfarsit, Stare = p.Stare })
            .ToListAsync(cancellationToken);
    }

    public async Task<ProiectDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _db.Proiecte.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return p == null ? null : new ProiectDto { Id = p.Id, Nume = p.Nume, Client = p.Client, DataStart = p.DataStart, DataSfarsit = p.DataSfarsit, Stare = p.Stare };
    }

    public async Task<ProiectDto> CreateAsync(CreateProiectRequest request, CancellationToken cancellationToken = default)
    {
        var p = new Proiect
        {
            Nume = request.Nume.Trim(),
            Client = request.Client.Trim(),
            DataStart = request.DataStart,
            DataSfarsit = request.DataSfarsit,
            Stare = request.Stare
        };
        _db.Proiecte.Add(p);
        await _db.SaveChangesAsync(cancellationToken);
        return new ProiectDto { Id = p.Id, Nume = p.Nume, Client = p.Client, DataStart = p.DataStart, DataSfarsit = p.DataSfarsit, Stare = p.Stare };
    }

    public async Task<ProiectDto?> UpdateAsync(int id, UpdateProiectRequest request, CancellationToken cancellationToken = default)
    {
        var p = await _db.Proiecte.FindAsync([id], cancellationToken);
        if (p == null) return null;
        if (request.Nume != null) p.Nume = request.Nume.Trim();
        if (request.Client != null) p.Client = request.Client.Trim();
        if (request.DataStart.HasValue) p.DataStart = request.DataStart.Value;
        if (request.DataSfarsit.HasValue) p.DataSfarsit = request.DataSfarsit;
        if (request.Stare != null) p.Stare = request.Stare;
        await _db.SaveChangesAsync(cancellationToken);
        return new ProiectDto { Id = p.Id, Nume = p.Nume, Client = p.Client, DataStart = p.DataStart, DataSfarsit = p.DataSfarsit, Stare = p.Stare };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _db.Proiecte.FindAsync([id], cancellationToken);
        if (p == null) return false;
        _db.Proiecte.Remove(p);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
