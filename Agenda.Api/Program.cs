using Agenda.Api.Data;
using Agenda.Api.Middleware;
using Agenda.Api.Services;
using Microsoft.EntityFrameworkCore;

const string PoliticaCorsAngular = "PermitirAngular";

var builder = WebApplication.CreateBuilder(args);

// Acceso a datos (EF Core + SQL Server).
builder.Services.AddDbContext<AgendaDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Lógica de negocio.
builder.Services.AddScoped<IContactoService, ContactoService>();

// CORS para que el frontend Angular pueda llamar a la API.
// Se permite el origen configurado en appsettings.json y los encabezados
// necesarios (incluido X-Api-Key).
var origenesPermitidos = builder.Configuration.GetSection("Cors:OrigenesPermitidos").Get<string[]>()
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy(PoliticaCorsAngular, politica =>
        politica.WithOrigins(origenesPermitidos)
                .AllowAnyMethod()
                .WithHeaders("Content-Type", ApiKeyMiddleware.NombreHeader));
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Orden importante: CORS antes del middleware de API Key para que el
// preflight OPTIONS reciba las cabeceras CORS correctas. Y el ApiKey
// antes de MapControllers para que valide todas las rutas de API.
app.UseCors(PoliticaCorsAngular);
app.UseApiKey();

app.UseAuthorization();
app.MapControllers();

app.Run();
