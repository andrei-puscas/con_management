namespace Backend.Entities;

public class Echipa
{
    public int Id { get; set; }
    public string Nume { get; set; } = string.Empty;
    public int? SefEchipaId { get; set; }

    public Angajat? SefEchipa { get; set; }
    public ICollection<Angajat> Angajati { get; set; } = new List<Angajat>();
    public ICollection<Lucrare> Lucrari { get; set; } = new List<Lucrare>();
}
