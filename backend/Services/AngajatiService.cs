using Backend.Data;
using Backend.DTOs.Angajat;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class AngajatiService : IAngajatiService
{
    private readonly AppDbContext _db;
    private readonly IUserService _userService;

    public AngajatiService(AppDbContext db, IUserService userService)
    {
        _db = db;
        _userService = userService;
    }

    public async Task<IEnumerable<AngajatDto>> GetAllAsync(int? echipaId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Angajat> q = _db.Angajati.AsNoTracking().Include(a => a.Echipa).Include(a => a.Utilizator);
        if (echipaId.HasValue)
            q = q.Where(a => a.EchipaId == echipaId.Value);
        return await q.OrderBy(a => a.Nume)
            .Select(a => new AngajatDto
            {
                Id = a.Id,
                EchipaId = a.EchipaId,
                EchipaNume = a.Echipa != null ? a.Echipa.Nume : null,
                Nume = a.Nume,
                Rol = a.Rol,
                Competente = a.Competente,
                HasUser = a.Utilizator != null,
                UserEmail = a.Utilizator != null ? a.Utilizator.Email : null
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AngajatDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var a = await _db.Angajati.AsNoTracking().Include(x => x.Echipa).Include(x => x.Utilizator).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return a == null ? null : new AngajatDto
        {
            Id = a.Id,
            EchipaId = a.EchipaId,
            EchipaNume = a.Echipa?.Nume,
            Nume = a.Nume,
            Rol = a.Rol,
            Competente = a.Competente,
            HasUser = a.Utilizator != null,
            UserEmail = a.Utilizator?.Email
        };
    }

    public async Task<AngajatDto> CreateAsync(CreateAngajatRequest request, CancellationToken cancellationToken = default)
    {
        if (request.EchipaId.HasValue && !await _db.Echipe.AnyAsync(e => e.Id == request.EchipaId.Value, cancellationToken))
            throw new InvalidOperationException("Echipă inexistentă.");

        var a = new Angajat { Nume = request.Nume.Trim(), Rol = request.Rol.Trim(), Competente = request.Competente?.Trim(), EchipaId = request.EchipaId };
        _db.Angajati.Add(a);
        await _db.SaveChangesAsync(cancellationToken);

        // Creare automată utilizator dacă se dorește
        string? userEmail = null;
        if (request.CreateUser)
        {
            try
            {
                var email = string.IsNullOrWhiteSpace(request.UserEmail)
                    ? GenerateEmailFromName(request.Nume)
                    : request.UserEmail;

                // Verifică dacă email-ul nu există deja
                if (await _db.Utilizatori.AnyAsync(u => u.Email == email, cancellationToken))
                    email = GenerateUniqueEmail(request.Nume, a.Id);

                var createUserRequest = new Backend.DTOs.User.CreateUserRequest
                {
                    Email = email,
                    Password = "Angajat123!", // Parolă implicită
                    Rol = "User"
                };

                var user = await _userService.CreateAsync(createUserRequest, cancellationToken);

                // Asociază utilizatorul cu angajatul
                user.AngajatId = a.Id;
                await _db.SaveChangesAsync(cancellationToken);

                userEmail = email;
            }
            catch
            {
                // Dacă crearea utilizatorului eșuează, continuăm fără utilizator
            }
        }

        var echipa = request.EchipaId.HasValue ? await _db.Echipe.FindAsync([request.EchipaId.Value], cancellationToken) : null;
        return new AngajatDto
        {
            Id = a.Id,
            EchipaId = a.EchipaId,
            EchipaNume = echipa?.Nume,
            Nume = a.Nume,
            Rol = a.Rol,
            Competente = a.Competente,
            HasUser = userEmail != null,
            UserEmail = userEmail
        };
    }

    private static string GenerateEmailFromName(string nume)
    {
        var parts = nume.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0]}.{parts[1]}@conmanagement.local";
        return $"{parts[0]}@conmanagement.local";
    }

    private static string GenerateUniqueEmail(string nume, int id)
    {
        var parts = nume.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0]}.{parts[1]}{id}@conmanagement.local";
        return $"{parts[0]}{id}@conmanagement.local";
    }

    public async Task<AngajatDto?> UpdateAsync(int id, UpdateAngajatRequest request, CancellationToken cancellationToken = default)
    {
        var a = await _db.Angajati.FindAsync([id], cancellationToken);
        if (a == null) return null;
        if (request.Nume != null) a.Nume = request.Nume.Trim();
        if (request.Rol != null) a.Rol = request.Rol.Trim();
        if (request.Competente != null) a.Competente = request.Competente.Trim();
        if (request.EchipaId.HasValue)
        {
            if (!await _db.Echipe.AnyAsync(e => e.Id == request.EchipaId.Value, cancellationToken))
                throw new InvalidOperationException("Echipă inexistentă.");
            a.EchipaId = request.EchipaId;
        }
        else if (request.EchipaId == null)
            a.EchipaId = null;
        await _db.SaveChangesAsync(cancellationToken);
        var echipa = a.EchipaId.HasValue ? await _db.Echipe.FindAsync([a.EchipaId.Value], cancellationToken) : null;
        return new AngajatDto { Id = a.Id, EchipaId = a.EchipaId, EchipaNume = echipa?.Nume, Nume = a.Nume, Rol = a.Rol, Competente = a.Competente };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var a = await _db.Angajati.FindAsync([id], cancellationToken);
        if (a == null) return false;
        _db.Angajati.Remove(a);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
