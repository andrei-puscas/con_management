namespace Backend.Entities;

public class Utilizator
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string ParolaHash { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty; // Admin, Manager, User
}
