namespace Backend.Entities;

public class Deviz
{
    public int Id { get; set; }
    public int ProiectId { get; set; }
    public string Titlu { get; set; } = string.Empty;
    public string? NumarInregistrare { get; set; }
    public string? Beneficiar { get; set; }
    public string? Executant { get; set; }
    public decimal CotaTVA { get; set; } = 19;
    public DateTime Data { get; set; } = DateTime.UtcNow;

    public Proiect? Proiect { get; set; }
    public ICollection<DevizLinie> Linii { get; set; } = new List<DevizLinie>();
}
