using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Santier;

public class UpdateSantierRequest
{
    public int? ProiectId { get; set; }

    [StringLength(500)]
    public string? Adresa { get; set; }

    [StringLength(1000)]
    public string? Descriere { get; set; }
}
