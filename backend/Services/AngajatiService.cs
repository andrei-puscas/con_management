using Backend.Data;
using Backend.DTOs.Angajat;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class AngajatiService : IAngajatiService
{
    private readonly AppDbContext _db;

    public AngajatiService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<AngajatDto>> GetAllAsync(int? echipaId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Angajat> q = _db.Angajati.AsNoTracking().Include(a => a.Echipa);
        if (echipaId.HasValue)
            q = q.Where(a => a.EchipaId == echipaId.Value);
        return await q.OrderBy(a => a.Nume)
            .Select(a => new AngajatDto { Id = a.Id, EchipaId = a.EchipaId, EchipaNume = a.Echipa != null ? a.Echipa.Nume : null, Nume = a.Nume, Rol = a.Rol, Competente = a.Competente })
            .ToListAsync(cancellationToken);
    }

    public async Task<AngajatDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var a = await _db.Angajati.AsNoTracking().Include(x => x.Echipa).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return a == null ? null : new AngajatDto { Id = a.Id, EchipaId = a.EchipaId, EchipaNume = a.Echipa?.Nume, Nume = a.Nume, Rol = a.Rol, Competente = a.Competente };
    }

    public async Task<AngajatDto> CreateAsync(CreateAngajatRequest request, CancellationToken cancellationToken = default)
    {
        if (request.EchipaId.HasValue && !await _db.Echipe.AnyAsync(e => e.Id == request.EchipaId.Value, cancellationToken))
            throw new InvalidOperationException("Echipă inexistentă.");
        var a = new Angajat { Nume = request.Nume.Trim(), Rol = request.Rol.Trim(), Competente = request.Competente?.Trim(), EchipaId = request.EchipaId };
        _db.Angajati.Add(a);
        await _db.SaveChangesAsync(cancellationToken);
        var echipa = request.EchipaId.HasValue ? await _db.Echipe.FindAsync([request.EchipaId.Value], cancellationToken) : null;
        return new AngajatDto { Id = a.Id, EchipaId = a.EchipaId, EchipaNume = echipa?.Nume, Nume = a.Nume, Rol = a.Rol, Competente = a.Competente };
    }

    public async Task<AngajatDto?> UpdateAsync(int id, UpdateAngajatRequest request, CancellationToken cancellationToken = default)
    {
        var a = await _db.Angajati.FindAsync([id], cancellationToken);
        if (a == null) return null;
        if (request.Nume != null) a.Nume = request.Nume.Trim();
        if (request.Rol != null) a.Rol = request.Rol.Trim();
        if (request.Competente != null) a.Competente = request.Competente.Trim();
        if (request.EchipaId.HasValue)
        {
            if (!await _db.Echipe.AnyAsync(e => e.Id == request.EchipaId.Value, cancellationToken))
                throw new InvalidOperationException("Echipă inexistentă.");
            a.EchipaId = request.EchipaId;
        }
        else if (request.EchipaId == null)
            a.EchipaId = null;
        await _db.SaveChangesAsync(cancellationToken);
        var echipa = a.EchipaId.HasValue ? await _db.Echipe.FindAsync([a.EchipaId.Value], cancellationToken) : null;
        return new AngajatDto { Id = a.Id, EchipaId = a.EchipaId, EchipaNume = echipa?.Nume, Nume = a.Nume, Rol = a.Rol, Competente = a.Competente };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var a = await _db.Angajati.FindAsync([id], cancellationToken);
        if (a == null) return false;
        _db.Angajati.Remove(a);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
