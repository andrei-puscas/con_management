namespace Backend.DTOs.ProiectFisier;

public class ProiectFisierDto
{
    public int Id { get; set; }
    public int ProiectId { get; set; }
    public int UtilizatorId { get; set; }
    public string? UtilizatorEmail { get; set; }
    public string NumeOriginal { get; set; } = string.Empty;
    public string TipFisier { get; set; } = string.Empty;
    public DateTime DataIncarcare { get; set; }
}
