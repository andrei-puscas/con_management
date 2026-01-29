using Backend.Data;
using Backend.DTOs.Santier;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class SantierService : ISantierService
{
    private readonly AppDbContext _db;

    public SantierService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<SantierDto>> GetAllAsync(int? userId = null, string? userRole = null, int? proiectId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Santier> q = _db.Santier.AsNoTracking();

        // Filtrare pentru utilizatori normali - doar santiere unde echipa lor lucrează
        if (userId.HasValue && userRole == "User")
        {
            var utilizator = await _db.Utilizatori
                .AsNoTracking()
                .Include(u => u.Angajat)
                .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

            if (utilizator?.Angajat?.EchipaId != null)
            {
                var echipaId = utilizator.Angajat.EchipaId.Value;
                q = q.Where(s => s.Lucrari.Any(l => l.Echipe.Any(e => e.Id == echipaId)));
            }
            else
            {
                return new List<SantierDto>();
            }
        }

        if (proiectId.HasValue)
            q = q.Where(s => s.ProiectId == proiectId.Value);

        return await q.OrderBy(s => s.Adresa)
            .Select(s => new SantierDto { Id = s.Id, ProiectId = s.ProiectId, Adresa = s.Adresa, Descriere = s.Descriere })
            .ToListAsync(cancellationToken);
    }

    public async Task<SantierDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var s = await _db.Santier.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return s == null ? null : new SantierDto { Id = s.Id, ProiectId = s.ProiectId, Adresa = s.Adresa, Descriere = s.Descriere };
    }

    public async Task<SantierDto> CreateAsync(CreateSantierRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _db.Proiecte.AnyAsync(p => p.Id == request.ProiectId, cancellationToken))
            throw new InvalidOperationException("Proiect inexistent.");
        var s = new Santier { ProiectId = request.ProiectId, Adresa = request.Adresa.Trim(), Descriere = request.Descriere?.Trim() };
        _db.Santier.Add(s);
        await _db.SaveChangesAsync(cancellationToken);
        return new SantierDto { Id = s.Id, ProiectId = s.ProiectId, Adresa = s.Adresa, Descriere = s.Descriere };
    }

    public async Task<SantierDto?> UpdateAsync(int id, UpdateSantierRequest request, CancellationToken cancellationToken = default)
    {
        var s = await _db.Santier.FindAsync([id], cancellationToken);
        if (s == null) return null;
        if (request.ProiectId.HasValue)
        {
            if (!await _db.Proiecte.AnyAsync(p => p.Id == request.ProiectId.Value, cancellationToken))
                throw new InvalidOperationException("Proiect inexistent.");
            s.ProiectId = request.ProiectId.Value;
        }
        if (request.Adresa != null) s.Adresa = request.Adresa.Trim();
        if (request.Descriere != null) s.Descriere = request.Descriere.Trim();
        await _db.SaveChangesAsync(cancellationToken);
        return new SantierDto { Id = s.Id, ProiectId = s.ProiectId, Adresa = s.Adresa, Descriere = s.Descriere };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var s = await _db.Santier.FindAsync([id], cancellationToken);
        if (s == null) return false;
        _db.Santier.Remove(s);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
