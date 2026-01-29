using Backend.DTOs.Santier;

namespace Backend.Services;

public interface ISantierService
{
    Task<IEnumerable<SantierDto>> GetAllAsync(int? proiectId = null, CancellationToken cancellationToken = default);
    Task<SantierDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SantierDto> CreateAsync(CreateSantierRequest request, CancellationToken cancellationToken = default);
    Task<SantierDto?> UpdateAsync(int id, UpdateSantierRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
