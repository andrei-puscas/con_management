using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Deviz;

public class CreateDevizLinieRequest
{
    public int Numar { get; set; }
    [Required] public string Descriere { get; set; } = string.Empty;
    [Required] public string UM { get; set; } = string.Empty;
    public decimal Cantitate { get; set; }
    public decimal PretUnitar { get; set; }
}

public class CreateDevizRequest
{
    [Required] public string Titlu { get; set; } = string.Empty;
    public string? NumarInregistrare { get; set; }
    public string? Beneficiar { get; set; }
    public string? Executant { get; set; }
    public decimal CotaTVA { get; set; } = 19;
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public List<CreateDevizLinieRequest> Linii { get; set; } = new();
}

public class UpdateDevizRequest
{
    public string? Titlu { get; set; }
    public string? NumarInregistrare { get; set; }
    public string? Beneficiar { get; set; }
    public string? Executant { get; set; }
    public decimal? CotaTVA { get; set; }
    public DateTime? Data { get; set; }
    public List<CreateDevizLinieRequest>? Linii { get; set; }
}
