using Backend.Data;
using Backend.DTOs.User;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Utilizatori
            .AsNoTracking()
            .Include(u => u.Angajat)
            .OrderBy(u => u.Email)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Rol = u.Rol,
                AngajatId = u.AngajatId,
                AngajatNume = u.Angajat != null ? u.Angajat.Nume : null
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var u = await _db.Utilizatori.AsNoTracking().Include(x => x.Angajat).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return u == null ? null : new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            Rol = u.Rol,
            AngajatId = u.AngajatId,
            AngajatNume = u.Angajat?.Nume
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await _db.Utilizatori.AnyAsync(x => x.Email == request.Email, cancellationToken))
            throw new InvalidOperationException("Există deja un utilizator cu acest email.");

        var user = new Utilizator
        {
            Email = request.Email.Trim(),
            ParolaHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Rol = request.Rol
        };
        _db.Utilizatori.Add(user);
        await _db.SaveChangesAsync(cancellationToken);
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Rol = user.Rol,
            AngajatId = user.AngajatId,
            AngajatNume = null
        };
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _db.Utilizatori.FindAsync([id], cancellationToken);
        if (user == null) return null;

        if (request.Email != null)
        {
            var trimmed = request.Email.Trim();
            if (trimmed != user.Email && await _db.Utilizatori.AnyAsync(x => x.Email == trimmed, cancellationToken))
                throw new InvalidOperationException("Există deja un utilizator cu acest email.");
            user.Email = trimmed;
        }
        if (request.Password != null)
            user.ParolaHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        if (request.Rol != null)
            user.Rol = request.Rol;

        await _db.SaveChangesAsync(cancellationToken);
        var angajat = user.AngajatId.HasValue ? await _db.Angajati.FindAsync([user.AngajatId.Value], cancellationToken) : null;
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Rol = user.Rol,
            AngajatId = user.AngajatId,
            AngajatNume = angajat?.Nume
        };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _db.Utilizatori.FindAsync([id], cancellationToken);
        if (user == null) return false;
        _db.Utilizatori.Remove(user);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
