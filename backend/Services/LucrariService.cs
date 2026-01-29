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
        IQueryable<Lucrare> q = _db.Lucrari.AsNoTracking().Include(l => l.Echipe);
        if (santierId.HasValue)
            q = q.Where(l => l.SantierId == santierId.Value);
        if (!string.IsNullOrWhiteSpace(stare))
            q = q.Where(l => l.Stare == stare);
        var list = await q.OrderBy(l => l.Termen).ToListAsync(cancellationToken);
        return list.Select(l => MapToDto(l));
    }

    public async Task<LucrareDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var l = await _db.Lucrari.AsNoTracking().Include(x => x.Echipe).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return l == null ? null : MapToDto(l);
    }

    public async Task<LucrareDto> CreateAsync(CreateLucrareRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _db.Santier.AnyAsync(s => s.Id == request.SantierId, cancellationToken))
            throw new InvalidOperationException("Șantier inexistent.");
        var echipaIds = request.EchipaIds ?? new List<int>();
        foreach (var eid in echipaIds)
        {
            if (!await _db.Echipe.AnyAsync(e => e.Id == eid, cancellationToken))
                throw new InvalidOperationException($"Echipă cu id {eid} inexistentă.");
        }
        var l = new Lucrare { SantierId = request.SantierId, Descriere = request.Descriere.Trim(), Termen = request.Termen, Stare = request.Stare };
        _db.Lucrari.Add(l);
        await _db.SaveChangesAsync(cancellationToken);
        if (echipaIds.Count > 0)
        {
            var echipe = await _db.Echipe.Where(e => echipaIds.Contains(e.Id)).ToListAsync(cancellationToken);
            l.Echipe = echipe;
            await _db.SaveChangesAsync(cancellationToken);
        }
        return MapToDto(l);
    }

    public async Task<LucrareDto?> UpdateAsync(int id, UpdateLucrareRequest request, CancellationToken cancellationToken = default)
    {
        var l = await _db.Lucrari.Include(x => x.Echipe).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (l == null) return null;
        if (request.SantierId.HasValue)
        {
            if (!await _db.Santier.AnyAsync(s => s.Id == request.SantierId.Value, cancellationToken))
                throw new InvalidOperationException("Șantier inexistent.");
            l.SantierId = request.SantierId.Value;
        }
        if (request.Descriere != null) l.Descriere = request.Descriere.Trim();
        if (request.Termen.HasValue) l.Termen = request.Termen.Value;
        if (request.Stare != null) l.Stare = request.Stare;
        if (request.EchipaIds != null)
        {
            foreach (var eid in request.EchipaIds)
            {
                if (!await _db.Echipe.AnyAsync(e => e.Id == eid, cancellationToken))
                    throw new InvalidOperationException($"Echipă cu id {eid} inexistentă.");
            }
            l.Echipe = await _db.Echipe.Where(e => request.EchipaIds.Contains(e.Id)).ToListAsync(cancellationToken);
        }
        await _db.SaveChangesAsync(cancellationToken);
        await _db.Entry(l).Collection(x => x.Echipe).LoadAsync(cancellationToken);
        return MapToDto(l);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var l = await _db.Lucrari.FindAsync([id], cancellationToken);
        if (l == null) return false;
        _db.Lucrari.Remove(l);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static LucrareDto MapToDto(Lucrare l)
    {
        var echipeNume = l.Echipe?.Count > 0 ? string.Join(", ", l.Echipe.Select(e => e.Nume)) : string.Empty;
        var echipaIds = l.Echipe?.Select(e => e.Id).ToList() ?? new List<int>();
        return new LucrareDto
        {
            Id = l.Id,
            SantierId = l.SantierId,
            EchipaIds = echipaIds,
            EchipeNume = echipeNume,
            Descriere = l.Descriere,
            Termen = l.Termen,
            Stare = l.Stare
        };
    }
}
