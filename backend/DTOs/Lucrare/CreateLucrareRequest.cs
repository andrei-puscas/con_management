using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Lucrare;

public class CreateLucrareRequest
{
    [Required(ErrorMessage = "SantierId obligatoriu")]
    public int SantierId { get; set; }

    /// <summary>Id-uri echipe asignate la lucrare (una sau mai multe).</summary>
    public List<int> EchipaIds { get; set; } = new();

    [Required(ErrorMessage = "Descriere obligatorie")]
    [StringLength(1000)]
    public string Descriere { get; set; } = string.Empty;

    [Required(ErrorMessage = "Termen obligatoriu")]
    public DateTime Termen { get; set; }

    [Required]
    [StringLength(50)]
    public string Stare { get; set; } = "Planificat";
}
