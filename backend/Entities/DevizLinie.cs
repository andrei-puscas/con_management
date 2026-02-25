namespace Backend.Entities;

public class DevizLinie
{
    public int Id { get; set; }
    public int DevizId { get; set; }
    public int Numar { get; set; }
    public string Descriere { get; set; } = string.Empty;
    public string UM { get; set; } = string.Empty;
    public decimal Cantitate { get; set; }
    public decimal PretUnitar { get; set; }

    public Deviz? Deviz { get; set; }
}
