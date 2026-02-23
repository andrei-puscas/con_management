using Backend.DTOs.ProiectComentariu;

namespace Backend.Services;

public interface IProiectComentariiService
{
    Task<IEnumerable<ProiectComentariuDto>> GetByProiectIdAsync(int proiectId, CancellationToken cancellationToken = default);
    Task<ProiectComentariuDto?> CreateAsync(int proiectId, int utilizatorId, CreateProiectComentariuRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int proiectId, int comentariuId, int? utilizatorId, string? userRole, CancellationToken cancellationToken = default);
}
