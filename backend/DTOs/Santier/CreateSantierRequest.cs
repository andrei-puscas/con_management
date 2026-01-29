using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Santier;

public class CreateSantierRequest
{
    [Required(ErrorMessage = "ProiectId obligatoriu")]
    public int ProiectId { get; set; }

    [Required(ErrorMessage = "Adresa obligatorie")]
    [StringLength(500)]
    public string Adresa { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Descriere { get; set; }
}
