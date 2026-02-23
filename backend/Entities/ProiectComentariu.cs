namespace Backend.Entities;

public class ProiectComentariu
{
    public int Id { get; set; }
    public int ProiectId { get; set; }
    public int UtilizatorId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime DataCreare { get; set; } = DateTime.UtcNow;

    public Proiect? Proiect { get; set; }
    public Utilizator? Utilizator { get; set; }
}
