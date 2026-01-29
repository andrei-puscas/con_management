namespace Backend.DTOs.Santier;

public class SantierDto
{
    public int Id { get; set; }
    public int ProiectId { get; set; }
    public string Adresa { get; set; } = string.Empty;
    public string? Descriere { get; set; }
}
