namespace Backend.DTOs.ProiectComentariu;

public class ProiectComentariuDto
{
    public int Id { get; set; }
    public int ProiectId { get; set; }
    public int UtilizatorId { get; set; }
    public string? UtilizatorEmail { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime DataCreare { get; set; }
}
