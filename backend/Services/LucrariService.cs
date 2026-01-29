using Backend.Data;
using Backend.DTOs.Lucrare;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class LucrariService : ILucrariService
{
    private readonly AppDbContext _db;

    public LucrariService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<LucrareDto>> GetAllAsync(int? santierId = null, string? stare = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Lucrare> q = _db.Lucrari.AsNoTracking().Include(l => l.Echipa);
        if (santierId.HasValue)
            q = q.Where(l => l.SantierId == santierId.Value);
        if (!string.IsNullOrWhiteSpace(stare))
            q = q.Where(l => l.Stare == stare);
        return await q.OrderBy(l => l.Termen)
            .Select(l => new LucrareDto { Id = l.Id, SantierId = l.SantierId, EchipaId = l.EchipaId, EchipaNume = l.Echipa != null ? l.Echipa.Nume : null, Descriere = l.Descriere, Termen = l.Termen, Stare = l.Stare })
            .ToListAsync(cancellationToken);
    }

    public async Task<LucrareDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var l = await _db.Lucrari.AsNoTracking().Include(x => x.Echipa).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return l == null ? null : new LucrareDto { Id = l.Id, SantierId = l.SantierId, EchipaId = l.EchipaId, EchipaNume = l.Echipa?.Nume, Descriere = l.Descriere, Termen = l.Termen, Stare = l.Stare };
    }

    public async Task<LucrareDto> CreateAsync(CreateLucrareRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _db.Santier.AnyAsync(s => s.Id == request.SantierId, cancellationToken))
            throw new InvalidOperationException("Șantier inexistent.");
        if (request.EchipaId.HasValue && !await _db.Echipe.AnyAsync(e => e.Id == request.EchipaId.Value, cancellationToken))
            throw new InvalidOperationException("Echipă inexistentă.");
        var l = new Lucrare { SantierId = request.SantierId, EchipaId = request.EchipaId, Descriere = request.Descriere.Trim(), Termen = request.Termen, Stare = request.Stare };
        _db.Lucrari.Add(l);
        await _db.SaveChangesAsync(cancellationToken);
        var echipa = request.EchipaId.HasValue ? await _db.Echipe.FindAsync([request.EchipaId.Value], cancellationToken) : null;
        return new LucrareDto { Id = l.Id, SantierId = l.SantierId, EchipaId = l.EchipaId, EchipaNume = echipa?.Nume, Descriere = l.Descriere, Termen = l.Termen, Stare = l.Stare };
    }

    public async Task<LucrareDto?> UpdateAsync(int id, UpdateLucrareRequest request, CancellationToken cancellationToken = default)
    {
        var l = await _db.Lucrari.FindAsync([id], cancellationToken);
        if (l == null) return null;
        if (request.SantierId.HasValue)
        {
            if (!await _db.Santier.AnyAsync(s => s.Id == request.SantierId.Value, cancellationToken))
                throw new InvalidOperationException("Șantier inexistent.");
            l.SantierId = request.SantierId.Value;
        }
        if (request.EchipaId.HasValue)
        {
            if (!await _db.Echipe.AnyAsync(e => e.Id == request.EchipaId.Value, cancellationToken))
                throw new InvalidOperationException("Echipă inexistentă.");
            l.EchipaId = request.EchipaId;
        }
        else if (request.EchipaId == null)
            l.EchipaId = null;
        if (request.Descriere != null) l.Descriere = request.Descriere.Trim();
        if (request.Termen.HasValue) l.Termen = request.Termen.Value;
        if (request.Stare != null) l.Stare = request.Stare;
        await _db.SaveChangesAsync(cancellationToken);
        var echipa = l.EchipaId.HasValue ? await _db.Echipe.FindAsync([l.EchipaId.Value], cancellationToken) : null;
        return new LucrareDto { Id = l.Id, SantierId = l.SantierId, EchipaId = l.EchipaId, EchipaNume = echipa?.Nume, Descriere = l.Descriere, Termen = l.Termen, Stare = l.Stare };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var l = await _db.Lucrari.FindAsync([id], cancellationToken);
        if (l == null) return false;
        _db.Lucrari.Remove(l);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
