using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Angajat;

public class CreateAngajatRequest
{
    [Required(ErrorMessage = "Nume obligatoriu")]
    [StringLength(200)]
    public string Nume { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rol obligatoriu")]
    [StringLength(100)]
    public string Rol { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Competente { get; set; }

    public int? EchipaId { get; set; }
}
