using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.User;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Email obligatoriu")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parola obligatorie")]
    [MinLength(6, ErrorMessage = "Parola minim 6 caractere")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rol obligatoriu")]
    [RegularExpression(@"^(Admin|Manager|User)$", ErrorMessage = "Rol: Admin, Manager sau User")]
    public string Rol { get; set; } = string.Empty;
}
