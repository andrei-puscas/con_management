# Backend – ConManagement API

.NET 8 Web API, Entity Framework Core (Code First), MS SQL Server Express, JWT.

## Configurare

- **Connection string**: `appsettings.json` – `ConnectionStrings:DefaultConnection`. Pentru SQL Server Express local: poți folosi `(localdb)\mssqllocaldb` sau `.\SQLEXPRESS`.
- **JWT**: cheia și issuer în `appsettings.json` – secțiunea `Jwt`. În producție folosește variabile de mediu.

## Comenzi

```bash
dotnet restore
dotnet build
dotnet run
```

API: http://localhost:5000  
Swagger: http://localhost:5000/swagger

## Faza 1

Proiect gol cu dependențe și configurare: JWT, CORS, Swagger, policy-uri (AdminOnly, ManagerOrAdmin). DbContext și controllere vor fi adăugate în Faza 2/3.
