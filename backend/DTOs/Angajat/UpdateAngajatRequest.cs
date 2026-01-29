using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Angajat;

public class UpdateAngajatRequest
{
    [StringLength(200)]
    public string? Nume { get; set; }

    [StringLength(100)]
    public string? Rol { get; set; }

    [StringLength(500)]
    public string? Competente { get; set; }

    public int? EchipaId { get; set; }
}
