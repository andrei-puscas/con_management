using Backend.Data;
using Backend.DTOs.ProiectComentariu;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class ProiectComentariiService : IProiectComentariiService
{
    private readonly AppDbContext _db;

    public ProiectComentariiService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ProiectComentariuDto>> GetByProiectIdAsync(int proiectId, CancellationToken cancellationToken = default)
    {
        return await _db.ProiectComentarii
            .AsNoTracking()
            .Include(c => c.Utilizator)
            .Where(c => c.ProiectId == proiectId)
            .OrderBy(c => c.DataCreare)
            .Select(c => new ProiectComentariuDto
            {
                Id = c.Id,
                ProiectId = c.ProiectId,
                UtilizatorId = c.UtilizatorId,
                UtilizatorEmail = c.Utilizator != null ? c.Utilizator.Email : null,
                Text = c.Text,
                DataCreare = c.DataCreare
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ProiectComentariuDto?> CreateAsync(int proiectId, int utilizatorId, CreateProiectComentariuRequest request, CancellationToken cancellationToken = default)
    {
        var proiectExists = await _db.Proiecte.AnyAsync(p => p.Id == proiectId, cancellationToken);
        if (!proiectExists) return null;

        var comentariu = new ProiectComentariu
        {
            ProiectId = proiectId,
            UtilizatorId = utilizatorId,
            Text = request.Text.Trim(),
            DataCreare = DateTime.UtcNow
        };
        _db.ProiectComentarii.Add(comentariu);
        await _db.SaveChangesAsync(cancellationToken);

        var utilizator = await _db.Utilizatori.AsNoTracking().FirstOrDefaultAsync(u => u.Id == utilizatorId, cancellationToken);
        return new ProiectComentariuDto
        {
            Id = comentariu.Id,
            ProiectId = comentariu.ProiectId,
            UtilizatorId = comentariu.UtilizatorId,
            UtilizatorEmail = utilizator?.Email,
            Text = comentariu.Text,
            DataCreare = comentariu.DataCreare
        };
    }

    public async Task<bool> DeleteAsync(int proiectId, int comentariuId, int? utilizatorId, string? userRole, CancellationToken cancellationToken = default)
    {
        var comentariu = await _db.ProiectComentarii
            .FirstOrDefaultAsync(c => c.Id == comentariuId && c.ProiectId == proiectId, cancellationToken);
        if (comentariu == null) return false;

        var isAdminOrManager = userRole is "Admin" or "Manager";
        var isOwner = utilizatorId.HasValue && comentariu.UtilizatorId == utilizatorId.Value;
        if (!isOwner && !isAdminOrManager) return false;

        _db.ProiectComentarii.Remove(comentariu);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
