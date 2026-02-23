using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.ProiectComentariu;

public class CreateProiectComentariuRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}
