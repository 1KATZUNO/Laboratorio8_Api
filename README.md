# Agenda.Api — Laboratorio 8

API REST en **ASP.NET Core 10** que expone los contactos de una agenda telefónica, protegida mediante **API Key** enviada en el encabezado HTTP `X-Api-Key`.

**Estudiante:** David González · **Carnet:** C23740
**Curso:** Lenguajes para Aplicaciones Comerciales (UCR)

> Todas las clases (Model, DA, BL) viven dentro del mismo proyecto API,
> organizadas en carpetas: `Models/`, `Data/`, `Services/`, `Middleware/`.

## Estructura

```
Agenda.Api.slnx
└── Agenda.Api/
    ├── Models/Contacto.cs              ← entidad (Id, Nombre, NumeroTelefonico)
    ├── Data/AgendaDbContext.cs         ← EF Core, mapeo a tabla Contactos
    ├── Services/IContactoService.cs    ← contrato BL
    ├── Services/ContactoService.cs     ← lógica
    ├── Middleware/ApiKeyMiddleware.cs  ← valida X-Api-Key
    ├── Controllers/ContactosController.cs
    ├── Migrations/                     ← migración inicial EF Core
    ├── Program.cs                      ← AddDbContext + AddScoped + CORS + UseApiKey
    └── appsettings.json                ← ConnectionString + ApiKey + Cors
```

## Requisitos

- .NET 10 SDK
- SQL Server (SQLEXPRESS o LocalDB)
- `dotnet tool restore` para instalar `dotnet-ef`

## Configuración

`appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=AgendaContactos;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
  },
  "ApiKey": {
    "Key": "AGENDA-LAB8-2026-CLAVE-SUPER-SECRETA"
  },
  "Cors": {
    "OrigenesPermitidos": [
      "http://localhost:4200",
      "http://127.0.0.1:4200",
      "http://localhost:49379",
      "http://127.0.0.1:49379"
    ]
  }
}
```

> Si tu instancia de SQL Server tiene otro nombre, editar la cadena
> de conexión. El proyecto está probado contra `LAPTOP-PUNNLDMT\SQLEXPRESS`.

## Levantar la API

```bash
dotnet tool restore
dotnet ef database update --project Agenda.Api/Agenda.Api.csproj
dotnet run              --project Agenda.Api/Agenda.Api.csproj
```

Por defecto queda en:
- `http://localhost:5260`
- `https://localhost:7253` (perfil https)

## Endpoints

Todos los endpoints requieren el encabezado `X-Api-Key`.

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Contactos` | Lista todos los contactos |
| GET | `/api/Contactos/{id}` | Detalle por Id |
| POST | `/api/Contactos` | Agrega un contacto (uso vía Postman) |

### Ejemplo curl

```bash
# Sin API Key → 401
curl -s -w "\nHTTP %{http_code}\n" http://localhost:5260/api/Contactos
# {"error":"Falta el encabezado X-Api-Key."}

# Con API Key → 200
curl -H "X-Api-Key: AGENDA-LAB8-2026-CLAVE-SUPER-SECRETA" \
     http://localhost:5260/api/Contactos
```

### Postman — agregar un contacto

```
POST http://localhost:5260/api/Contactos
Headers:
  Content-Type: application/json
  X-Api-Key:    AGENDA-LAB8-2026-CLAVE-SUPER-SECRETA
Body (raw JSON):
{
  "nombre": "Juan Pérez",
  "numeroTelefonico": "8888-9999"
}
```

## Middleware de API Key (resumen)

`ApiKeyMiddleware` lee `X-Api-Key`, lo compara con `ApiKey:Key` de la
configuración y responde:

- `401 Unauthorized` si falta o no coincide.
- `next()` si es válida (deja pasar la solicitud al pipeline).

Las solicitudes `OPTIONS` (preflight CORS) se dejan pasar sin validar
porque el navegador no envía encabezados personalizados en preflight.

## Frontend asociado

[Agenda.Angular (Laboratorio 8)](https://github.com/1KATZUNO/Laboratorio8_Angular) — consume `/api/Contactos` con el header `X-Api-Key`.
