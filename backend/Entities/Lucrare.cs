namespace Backend.Entities;

public class Lucrare
{
    public int Id { get; set; }
    public int SantierId { get; set; }
    public string Descriere { get; set; } = string.Empty;
    public DateTime Termen { get; set; }
    public string Stare { get; set; } = string.Empty; // ex: Planificat, În lucru, Finalizat

    public Santier Santier { get; set; } = null!;
    /// <summary>Una sau mai multe echipe asignate la această lucrare.</summary>
    public ICollection<Echipa> Echipe { get; set; } = new List<Echipa>();
}
