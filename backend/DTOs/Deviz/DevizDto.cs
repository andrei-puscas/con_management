namespace Backend.DTOs.Deviz;

public class DevizLinieDto
{
    public int Id { get; set; }
    public int Numar { get; set; }
    public string Descriere { get; set; } = string.Empty;
    public string UM { get; set; } = string.Empty;
    public decimal Cantitate { get; set; }
    public decimal PretUnitar { get; set; }
    public decimal Total => Cantitate * PretUnitar;
}

public class DevizDto
{
    public int Id { get; set; }
    public int ProiectId { get; set; }
    public string NumeProiect { get; set; } = string.Empty;
    public string Titlu { get; set; } = string.Empty;
    public string? NumarInregistrare { get; set; }
    public string? Beneficiar { get; set; }
    public string? Executant { get; set; }
    public decimal CotaTVA { get; set; }
    public DateTime Data { get; set; }
    public List<DevizLinieDto> Linii { get; set; } = new();
    public decimal TotalGeneral => Linii.Sum(l => l.Total);
}
