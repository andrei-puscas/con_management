using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Email obligatoriu")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parola obligatorie")]
    public string Password { get; set; } = string.Empty;
}
