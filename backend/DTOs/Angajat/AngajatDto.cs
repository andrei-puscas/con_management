namespace Backend.DTOs.Angajat;

public class AngajatDto
{
    public int Id { get; set; }
    public int? EchipaId { get; set; }
    public string? EchipaNume { get; set; }
    public string Nume { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string? Competente { get; set; }
    public bool HasUser { get; set; }
    public string? UserEmail { get; set; }
}
