namespace Backend.Entities;

public class Angajat
{
    public int Id { get; set; }
    public int? EchipaId { get; set; }
    public string Nume { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty; // ex: Sef de echipa, Muncitor
    public string? Competente { get; set; }

    public Echipa? Echipa { get; set; }
}
