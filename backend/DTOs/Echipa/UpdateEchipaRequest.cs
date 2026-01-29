using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Echipa;

public class UpdateEchipaRequest
{
    [StringLength(200)]
    public string? Nume { get; set; }

    public int? SefEchipaId { get; set; }
}
