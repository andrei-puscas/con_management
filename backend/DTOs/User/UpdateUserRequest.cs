using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.User;

public class UpdateUserRequest
{
    [EmailAddress]
    public string? Email { get; set; }

    [MinLength(6, ErrorMessage = "Parola minim 6 caractere")]
    public string? Password { get; set; }

    [RegularExpression(@"^(Admin|Manager|User)$", ErrorMessage = "Rol: Admin, Manager sau User")]
    public string? Rol { get; set; }

    public int? AngajatId { get; set; }
}
