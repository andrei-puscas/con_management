using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Deviz;

public class CreateDevizGlobalRequest
{
    [Required] public int ProiectId { get; set; }
    [Required] public string Titlu { get; set; } = string.Empty;
    public string? NumarInregistrare { get; set; }
    public string? Beneficiar { get; set; }
    public string? Executant { get; set; }
    public decimal CotaTVA { get; set; } = 19;
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public List<CreateDevizLinieRequest> Linii { get; set; } = new();
}
