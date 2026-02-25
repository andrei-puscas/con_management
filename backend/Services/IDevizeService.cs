using Backend.DTOs.Deviz;

namespace Backend.Services;

public interface IDevizeService
{
    Task<List<DevizDto>> GetAllAsync();
    Task<List<DevizDto>> GetByProiectIdAsync(int proiectId);
    Task<DevizDto?> GetByIdAsync(int id);
    Task<DevizDto> CreateAsync(int proiectId, CreateDevizRequest request);
    Task<DevizDto?> UpdateAsync(int id, UpdateDevizRequest request);
    Task<bool> DeleteAsync(int id);
    Task<byte[]> GeneratePdfAsync(int id);
}
