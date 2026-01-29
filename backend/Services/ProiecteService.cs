using Backend.Data;
using Backend.DTOs.Proiect;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class ProiecteService : IProiecteService
{
    private readonly AppDbContext _db;

    public ProiecteService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ProiectDto>> GetAllAsync(int? userId = null, string? userRole = null, string? stare = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Proiect> q = _db.Proiecte.AsNoTracking();

        // Filtrare pentru utilizatori normali - doar proiecte unde echipa lor lucrează
        if (userId.HasValue && userRole == "User")
        {
            var utilizator = await _db.Utilizatori
                .AsNoTracking()
                .Include(u => u.Angajat)
                .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

            if (utilizator?.Angajat?.EchipaId != null)
            {
                var echipaId = utilizator.Angajat.EchipaId.Value;
                // Proiecte care au santiere cu lucrări unde echipa utilizatorului participă
                q = q.Where(p => p.Santier.Any(s => s.Lucrari.Any(l => l.Echipe.Any(e => e.Id == echipaId))));
            }
            else
            {
                // Dacă utilizatorul nu are angajat sau angajatul nu are echipă, nu vede nimic
                return new List<ProiectDto>();
            }
        }

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
