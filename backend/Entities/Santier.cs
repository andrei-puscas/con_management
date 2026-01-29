namespace Backend.Entities;

public class Santier
{
    public int Id { get; set; }
    public int ProiectId { get; set; }
    public string Adresa { get; set; } = string.Empty;
    public string? Descriere { get; set; }

    public Proiect Proiect { get; set; } = null!;
    public ICollection<Lucrare> Lucrari { get; set; } = new List<Lucrare>();
}
