using Backend.DTOs.Proiect;

namespace Backend.Services;

public interface IProiecteService
{
    Task<IEnumerable<ProiectDto>> GetAllAsync(string? stare = null, CancellationToken cancellationToken = default);
    Task<ProiectDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProiectDto> CreateAsync(CreateProiectRequest request, CancellationToken cancellationToken = default);
    Task<ProiectDto?> UpdateAsync(int id, UpdateProiectRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
