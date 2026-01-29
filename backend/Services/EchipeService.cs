using Backend.Data;
using Backend.DTOs.Echipa;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class EchipeService : IEchipeService
{
    private readonly AppDbContext _db;

    public EchipeService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<EchipaDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Echipe
            .AsNoTracking()
            .Include(e => e.SefEchipa)
            .Select(e => new EchipaDto
            {
                Id = e.Id,
                Nume = e.Nume,
                SefEchipaId = e.SefEchipaId,
                SefEchipaNume = e.SefEchipa != null ? e.SefEchipa.Nume : null,
                NrAngajati = e.Angajati.Count
            })
            .OrderBy(e => e.Nume)
            .ToListAsync(cancellationToken);
    }

    public async Task<EchipaDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Echipe.AsNoTracking()
            .Include(x => x.SefEchipa)
            .Include(x => x.Angajati)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return e == null ? null : new EchipaDto
        {
            Id = e.Id,
            Nume = e.Nume,
            SefEchipaId = e.SefEchipaId,
            SefEchipaNume = e.SefEchipa?.Nume,
            NrAngajati = e.Angajati.Count
        };
    }

    public async Task<EchipaDto> CreateAsync(CreateEchipaRequest request, CancellationToken cancellationToken = default)
    {
        if (request.SefEchipaId.HasValue && !await _db.Angajati.AnyAsync(a => a.Id == request.SefEchipaId.Value, cancellationToken))
            throw new InvalidOperationException("Angajat (șef echipă) inexistent.");
        var e = new Echipa { Nume = request.Nume.Trim(), SefEchipaId = request.SefEchipaId };
        _db.Echipe.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        var sef = request.SefEchipaId.HasValue ? await _db.Angajati.FindAsync([request.SefEchipaId.Value], cancellationToken) : null;
        return new EchipaDto { Id = e.Id, Nume = e.Nume, SefEchipaId = e.SefEchipaId, SefEchipaNume = sef?.Nume, NrAngajati = 0 };
    }

    public async Task<EchipaDto?> UpdateAsync(int id, UpdateEchipaRequest request, CancellationToken cancellationToken = default)
    {
        var e = await _db.Echipe.FindAsync([id], cancellationToken);
        if (e == null) return null;
        if (request.Nume != null) e.Nume = request.Nume.Trim();
        if (request.SefEchipaId.HasValue)
        {
            if (!await _db.Angajati.AnyAsync(a => a.Id == request.SefEchipaId.Value, cancellationToken))
                throw new InvalidOperationException("Angajat (șef echipă) inexistent.");
            e.SefEchipaId = request.SefEchipaId;
        }
        else if (request.SefEchipaId == null)
            e.SefEchipaId = null;
        await _db.SaveChangesAsync(cancellationToken);
        var sef = e.SefEchipaId.HasValue ? await _db.Angajati.FindAsync([e.SefEchipaId.Value], cancellationToken) : null;
        var nr = await _db.Echipe.Where(x => x.Id == id).Select(x => x.Angajati.Count).FirstOrDefaultAsync(cancellationToken);
        return new EchipaDto { Id = e.Id, Nume = e.Nume, SefEchipaId = e.SefEchipaId, SefEchipaNume = sef?.Nume, NrAngajati = nr };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Echipe.FindAsync([id], cancellationToken);
        if (e == null) return false;
        _db.Echipe.Remove(e);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
