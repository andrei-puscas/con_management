using Backend.Data;
using Backend.DTOs.ProiectFisier;
using Backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class ProiectFisiereService : IProiectFisiereService
{
    private readonly AppDbContext _db;

    public ProiectFisiereService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ProiectFisierDto>> GetByProiectIdAsync(int proiectId, CancellationToken ct = default)
    {
        return await _db.ProiectFisiere
            .AsNoTracking()
            .Include(f => f.Utilizator)
            .Where(f => f.ProiectId == proiectId)
            .OrderByDescending(f => f.DataIncarcare)
            .Select(f => new ProiectFisierDto
            {
                Id = f.Id,
                ProiectId = f.ProiectId,
                UtilizatorId = f.UtilizatorId,
                UtilizatorEmail = f.Utilizator != null ? f.Utilizator.Email : null,
                NumeOriginal = f.NumeOriginal,
                TipFisier = f.TipFisier,
                DataIncarcare = f.DataIncarcare
            })
            .ToListAsync(ct);
    }

    public async Task<ProiectFisierDto?> UploadAsync(int proiectId, int utilizatorId, IFormFile file, CancellationToken ct = default)
    {
        var proiectExists = await _db.Proiecte.AnyAsync(p => p.Id == proiectId, ct);
        if (!proiectExists) return null;

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);

        var fisier = new ProiectFisier
        {
            ProiectId = proiectId,
            UtilizatorId = utilizatorId,
            NumeOriginal = file.FileName,
            TipFisier = file.ContentType ?? "application/octet-stream",
            Continut = ms.ToArray(),
            DataIncarcare = DateTime.UtcNow
        };
        _db.ProiectFisiere.Add(fisier);
        await _db.SaveChangesAsync(ct);

        var utilizator = await _db.Utilizatori.AsNoTracking().FirstOrDefaultAsync(u => u.Id == utilizatorId, ct);
        return new ProiectFisierDto
        {
            Id = fisier.Id,
            ProiectId = fisier.ProiectId,
            UtilizatorId = fisier.UtilizatorId,
            UtilizatorEmail = utilizator?.Email,
            NumeOriginal = fisier.NumeOriginal,
            TipFisier = fisier.TipFisier,
            DataIncarcare = fisier.DataIncarcare
        };
    }

    public async Task<(byte[] continut, string numeOriginal, string tipFisier)?> GetFileForDownloadAsync(int proiectId, int fisierId, CancellationToken ct = default)
    {
        var fisier = await _db.ProiectFisiere
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == fisierId && f.ProiectId == proiectId, ct);
        if (fisier == null) return null;
        return (fisier.Continut, fisier.NumeOriginal, fisier.TipFisier);
    }

    public async Task<bool> DeleteAsync(int proiectId, int fisierId, int? utilizatorId, string? userRole, CancellationToken ct = default)
    {
        var fisier = await _db.ProiectFisiere
            .FirstOrDefaultAsync(f => f.Id == fisierId && f.ProiectId == proiectId, ct);
        if (fisier == null) return false;

        var isOwner = utilizatorId.HasValue && fisier.UtilizatorId == utilizatorId.Value;
        if (!isOwner) return false;

        _db.ProiectFisiere.Remove(fisier);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
