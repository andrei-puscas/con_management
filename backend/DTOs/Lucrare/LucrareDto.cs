namespace Backend.DTOs.Lucrare;

public class LucrareDto
{
    public int Id { get; set; }
    public int SantierId { get; set; }
    public List<int> EchipaIds { get; set; } = new();
    public string EchipeNume { get; set; } = string.Empty; // ex: "Echipa A, Echipa B"
    public string Descriere { get; set; } = string.Empty;
    public DateTime Termen { get; set; }
    public string Stare { get; set; } = string.Empty;
}
