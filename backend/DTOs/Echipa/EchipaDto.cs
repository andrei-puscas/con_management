namespace Backend.DTOs.Echipa;

public class EchipaDto
{
    public int Id { get; set; }
    public string Nume { get; set; } = string.Empty;
    public int? SefEchipaId { get; set; }
    public string? SefEchipaNume { get; set; }
    public int NrAngajati { get; set; }
}
