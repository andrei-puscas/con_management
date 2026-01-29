# Backend – ConManagement API

.NET 10 Web API, Entity Framework Core (Code First), MS SQL Server Express, JWT.

## Configurare

- **Connection string**: `appsettings.json` – `ConnectionStrings:DefaultConnection`. Pentru SQL Server Express local: `.\SQLEXPRESS` (instanță default). Asigură-te că serviciul SQL Server (SQLEXPRESS) rulează.
- **JWT**: cheia și issuer în `appsettings.json` – secțiunea `Jwt`. În producție folosește variabile de mediu.

## Prima rulare (migrații + seed)

Schema conține doar **Utilizator** (auth). Dacă ai deja un folder `Migrations` cu o migrare veche, șterge-l și creează din nou.

```bash
dotnet restore
dotnet ef migrations add InitialCreate
dotnet run
```

La primul `dotnet run` se aplică migrațiile și se creează un utilizator **admin** dacă nu există niciun utilizator în baza de date.

**Cont admin (doar pentru development):**
- Email: `admin@conmanagement.local`
- Parolă: `Admin123!`
- Rol: Admin

## Comenzi uzuale

```bash
dotnet build
dotnet run
```

API: http://localhost:5000  
Swagger: http://localhost:5000/api/swagger

## Autentificare (subfaza curentă)

- **POST** `/api/auth/login` – body JSON: `{ "email": "...", "password": "..." }`.  
  Răspuns: `{ "token", "expires", "email", "role" }`.  
  Tokenul se trimite în header: `Authorization: Bearer <token>`.

Controllere CRUD (Users, Proiecte, Santier, Echipe, Angajați, Lucrări) vor fi adăugate în subfazele următoare.
