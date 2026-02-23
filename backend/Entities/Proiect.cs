namespace Backend.Entities;

public class Proiect
{
    public int Id { get; set; }
    public string Nume { get; set; } = string.Empty;
    public string Client { get; set; } = string.Empty;
    public DateTime DataStart { get; set; }
    public DateTime? DataSfarsit { get; set; }
    public string Stare { get; set; } = string.Empty; // ex: Activ, Încheiat
    public decimal? Buget { get; set; }
    public string? Moneda { get; set; } // RON, EUR

    public ICollection<Santier> Santier { get; set; } = new List<Santier>();
    public ICollection<ProiectComentariu> Comentarii { get; set; } = new List<ProiectComentariu>();
}
