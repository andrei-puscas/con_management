using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Lucrare;

public class UpdateLucrareRequest
{
    public int? SantierId { get; set; }
    public List<int>? EchipaIds { get; set; }

    [StringLength(1000)]
    public string? Descriere { get; set; }

    public DateTime? Termen { get; set; }

    [StringLength(50)]
    public string? Stare { get; set; }
}
