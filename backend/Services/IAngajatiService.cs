using Backend.DTOs.Angajat;

namespace Backend.Services;

public interface IAngajatiService
{
    Task<IEnumerable<AngajatDto>> GetAllAsync(int? echipaId = null, CancellationToken cancellationToken = default);
    Task<AngajatDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AngajatDto> CreateAsync(CreateAngajatRequest request, CancellationToken cancellationToken = default);
    Task<AngajatDto?> UpdateAsync(int id, UpdateAngajatRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
