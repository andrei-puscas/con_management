using Backend.Data;
using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
});
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProiecteService, ProiecteService>();
builder.Services.AddScoped<ISantierService, SantierService>();
builder.Services.AddScoped<IEchipeService, EchipeService>();
builder.Services.AddScoped<IAngajatiService, AngajatiService>();
builder.Services.AddScoped<ILucrariService, LucrariService>();
builder.Services.AddScoped<IProiectComentariiService, ProiectComentariiService>();
builder.Services.AddScoped<IProiectFisiereService, ProiectFisiereService>();
builder.Services.AddScoped<IDevizeService, DevizeService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ConManagement API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigins"] ?? "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "CheieSecretaFoarteLungaPentruDevelopmentDoar";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ConManagement";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = ClaimTypes.Role // tipul claim-ului de rol din JWT (URI complet)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Admin", "Manager"));
});

var app = builder.Build();

// Aplicare migrații, asigurare coloane Buget/Moneda pe Proiecte, seed admin (dacă nu există utilizatori)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    // Asigură coloanele Buget și Moneda pe Proiecte (dacă migrația nu le-a creat)
    await db.Database.ExecuteSqlRawAsync(@"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'Proiecte') AND name = N'Buget')
    ALTER TABLE [Proiecte] ADD [Buget] decimal(18,2) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'Proiecte') AND name = N'Moneda')
    ALTER TABLE [Proiecte] ADD [Moneda] nvarchar(10) NULL;
");

    // Creează tabelul ProiectComentarii (comentarii / idei pe proiect) dacă nu există
    await db.Database.ExecuteSqlRawAsync(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'ProiectComentarii')
BEGIN
    CREATE TABLE [ProiectComentarii] (
        [Id] int NOT NULL IDENTITY(1,1),
        [ProiectId] int NOT NULL,
        [UtilizatorId] int NOT NULL,
        [Text] nvarchar(2000) NOT NULL,
        [DataCreare] datetime2 NOT NULL,
        CONSTRAINT [PK_ProiectComentarii] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProiectComentarii_Proiecte_ProiectId] FOREIGN KEY ([ProiectId]) REFERENCES [Proiecte] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProiectComentarii_Utilizatori_UtilizatorId] FOREIGN KEY ([UtilizatorId]) REFERENCES [Utilizatori] ([Id]) ON DELETE NO ACTION
    );
    CREATE INDEX [IX_ProiectComentarii_ProiectId] ON [ProiectComentarii] ([ProiectId]);
    CREATE INDEX [IX_ProiectComentarii_UtilizatorId] ON [ProiectComentarii] ([UtilizatorId]);
END
");

    // Recreează tabelul ProiectFisiere cu schema nouă (fișiere stocate în DB ca varbinary)
    await db.Database.ExecuteSqlRawAsync(@"
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = N'ProiectFisiere')
    DROP TABLE [ProiectFisiere];

CREATE TABLE [ProiectFisiere] (
    [Id] int NOT NULL IDENTITY(1,1),
    [ProiectId] int NOT NULL,
    [UtilizatorId] int NOT NULL,
    [NumeOriginal] nvarchar(500) NOT NULL,
    [TipFisier] nvarchar(200) NOT NULL,
    [Continut] varbinary(max) NOT NULL,
    [DataIncarcare] datetime2 NOT NULL,
    CONSTRAINT [PK_ProiectFisiere] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProiectFisiere_Proiecte_ProiectId] FOREIGN KEY ([ProiectId]) REFERENCES [Proiecte] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProiectFisiere_Utilizatori_UtilizatorId] FOREIGN KEY ([UtilizatorId]) REFERENCES [Utilizatori] ([Id]) ON DELETE NO ACTION
);
CREATE INDEX [IX_ProiectFisiere_ProiectId] ON [ProiectFisiere] ([ProiectId]);
CREATE INDEX [IX_ProiectFisiere_UtilizatorId] ON [ProiectFisiere] ([UtilizatorId]);
");

    // Recreează tabelele Devize și DevizLinii cu schema actualizată
    await db.Database.ExecuteSqlRawAsync(@"
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = N'DevizLinii')
    DROP TABLE [DevizLinii];
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Devize')
    DROP TABLE [Devize];

CREATE TABLE [Devize] (
    [Id] int NOT NULL IDENTITY(1,1),
    [ProiectId] int NOT NULL,
    [Titlu] nvarchar(500) NOT NULL,
    [NumarInregistrare] nvarchar(100) NULL,
    [Beneficiar] nvarchar(500) NULL,
    [Executant] nvarchar(500) NULL,
    [CotaTVA] decimal(5,2) NOT NULL DEFAULT 19,
    [Data] datetime2 NOT NULL,
    CONSTRAINT [PK_Devize] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Devize_Proiecte_ProiectId] FOREIGN KEY ([ProiectId]) REFERENCES [Proiecte] ([Id]) ON DELETE CASCADE
);
CREATE INDEX [IX_Devize_ProiectId] ON [Devize] ([ProiectId]);

CREATE TABLE [DevizLinii] (
    [Id] int NOT NULL IDENTITY(1,1),
    [DevizId] int NOT NULL,
    [Numar] int NOT NULL,
    [Descriere] nvarchar(1000) NOT NULL,
    [UM] nvarchar(50) NOT NULL,
    [Cantitate] decimal(18,4) NOT NULL,
    [PretUnitar] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_DevizLinii] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DevizLinii_Devize_DevizId] FOREIGN KEY ([DevizId]) REFERENCES [Devize] ([Id]) ON DELETE CASCADE
);
CREATE INDEX [IX_DevizLinii_DevizId] ON [DevizLinii] ([DevizId]);
");

    if (!await db.Utilizatori.AnyAsync())
    {
        db.Utilizatori.Add(new Utilizator
        {
            Email = "admin@conmanagement.local",
            ParolaHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Rol = "Admin"
        });
        await db.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c => c.RouteTemplate = "api/swagger/{documentName}/swagger.json");
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "ConManagement API v1");
        c.RoutePrefix = "api/swagger";
    });
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
