namespace Backend.DTOs.Lucrare;

public class LucrareDto
{
    public int Id { get; set; }
    public int SantierId { get; set; }
    public int? EchipaId { get; set; }
    public string? EchipaNume { get; set; }
    public string Descriere { get; set; } = string.Empty;
    public DateTime Termen { get; set; }
    public string Stare { get; set; } = string.Empty;
}
