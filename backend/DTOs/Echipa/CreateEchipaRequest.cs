using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Echipa;

public class CreateEchipaRequest
{
    [Required(ErrorMessage = "Nume obligatoriu")]
    [StringLength(200)]
    public string Nume { get; set; } = string.Empty;

    public int? SefEchipaId { get; set; }
}
