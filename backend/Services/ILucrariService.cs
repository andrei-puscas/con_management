using Backend.DTOs.Lucrare;

namespace Backend.Services;

public interface ILucrariService
{
    Task<IEnumerable<LucrareDto>> GetAllAsync(int? santierId = null, string? stare = null, CancellationToken cancellationToken = default);
    Task<LucrareDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LucrareDto> CreateAsync(CreateLucrareRequest request, CancellationToken cancellationToken = default);
    Task<LucrareDto?> UpdateAsync(int id, UpdateLucrareRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
