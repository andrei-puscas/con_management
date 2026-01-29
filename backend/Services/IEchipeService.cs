using Backend.DTOs.Echipa;

namespace Backend.Services;

public interface IEchipeService
{
    Task<IEnumerable<EchipaDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EchipaDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EchipaDto> CreateAsync(CreateEchipaRequest request, CancellationToken cancellationToken = default);
    Task<EchipaDto?> UpdateAsync(int id, UpdateEchipaRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
