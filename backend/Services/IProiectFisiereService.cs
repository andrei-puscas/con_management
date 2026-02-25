using Backend.DTOs.ProiectFisier;
using Microsoft.AspNetCore.Http;

namespace Backend.Services;

public interface IProiectFisiereService
{
    Task<IEnumerable<ProiectFisierDto>> GetByProiectIdAsync(int proiectId, CancellationToken ct = default);
    Task<ProiectFisierDto?> UploadAsync(int proiectId, int utilizatorId, IFormFile file, CancellationToken ct = default);
    Task<(byte[] continut, string numeOriginal, string tipFisier)?> GetFileForDownloadAsync(int proiectId, int fisierId, CancellationToken ct = default);
    Task<bool> DeleteAsync(int proiectId, int fisierId, int? utilizatorId, string? userRole, CancellationToken ct = default);
}
