namespace Backend.Entities;

public class ProiectFisier
{
    public int Id { get; set; }
    public int ProiectId { get; set; }
    public int UtilizatorId { get; set; }
    public string NumeOriginal { get; set; } = string.Empty; // ex: contract.pdf
    public string TipFisier { get; set; } = string.Empty;    // MIME type
    public byte[] Continut { get; set; } = Array.Empty<byte>(); // continut fisier in DB
    public DateTime DataIncarcare { get; set; } = DateTime.UtcNow;

    public Proiect? Proiect { get; set; }
    public Utilizator? Utilizator { get; set; }
}
