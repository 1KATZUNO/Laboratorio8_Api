# Guía de configuración — Agenda.Api + Agenda.Angular

Pasos completos para levantar el laboratorio desde cero en una máquina nueva.

---

## 1. Prerrequisitos

| Software | Versión mínima | Cómo verificar |
|---|---|---|
| .NET SDK | 10.0 | `dotnet --version` |
| Node.js | 20.x | `node --version` |
| npm | 10.x | `npm --version` |
| SQL Server | Express 2019+ o LocalDB | `sqlcmd -?` debería existir |
| Postman | cualquiera | opcional, para cargar contactos manualmente |

---

## 2. Crear la base de datos

Hay **dos formas** equivalentes, elige la que prefieras.

### Opción A — Ejecutar el script SQL incluido (recomendada)

El script `script_sql/agenda.sql` crea la base, la tabla y 5 contactos de ejemplo.

```powershell
# Desde la raíz del repo Agenda.Api:
sqlcmd -S .\SQLEXPRESS -E -C -i script_sql\agenda.sql
```

Si tu instancia de SQL Server tiene otro nombre, reemplaza `.\SQLEXPRESS`:

```powershell
# Ejemplos:
sqlcmd -S (local)\SQLEXPRESS         -E -C -i script_sql\agenda.sql
sqlcmd -S LAPTOP-XYZ\SQLEXPRESS      -E -C -i script_sql\agenda.sql
sqlcmd -S (localdb)\MSSQLLocalDB     -E -C -i script_sql\agenda.sql
```

También puedes abrir `script_sql/agenda.sql` en SSMS o Azure Data Studio y darle F5.

### Opción B — Usar EF Core Migrations

```powershell
dotnet tool restore
dotnet ef database update --project Agenda.Api\Agenda.Api.csproj
```

EF Core leerá la cadena de conexión de `appsettings.json` y aplicará la migración `CreacionInicial`.

> 💡 La opción A es preferible si quieres que el profesor reproduzca rápido sin instalar `dotnet-ef`.

---

## 3. Configurar la cadena de conexión

Editar `Agenda.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=AgendaContactos;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
  }
}
```

Reemplaza `.\\SQLEXPRESS` por el nombre de tu instancia. Ejemplos comunes:

| Instancia | Cadena |
|---|---|
| SQL Server Express local | `Server=.\\SQLEXPRESS;Database=AgendaContactos;...` |
| LocalDB | `Server=(localdb)\\MSSQLLocalDB;Database=AgendaContactos;...` |
| SQL Server por nombre | `Server=NOMBRE-PC\\SQLEXPRESS;Database=AgendaContactos;...` |

---

## 4. Configurar la API Key

La clave que el cliente debe enviar en el encabezado `X-Api-Key` se define en `Agenda.Api/appsettings.json`:

```json
{
  "ApiKey": {
    "Key": "AGENDA-LAB8-2026-CLAVE-SUPER-SECRETA"
  }
}
```

⚠️ **Si la cambias acá, también debes cambiarla en el frontend Angular**
(`Agenda.Angular/src/environments/environment.ts`). Las dos cadenas deben coincidir exactamente.

---

## 5. Levantar la API

```powershell
# Desde la raíz de Agenda.Api:
dotnet run --project Agenda.Api\Agenda.Api.csproj
```

Salida esperada:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5260
      Now listening on: https://localhost:7253
info: Microsoft.Hosting.Lifetime[0]
      Application started.
```

### Probar con curl

```powershell
# Sin API Key — debe responder 401
curl http://localhost:5260/api/Contactos

# Con API Key — debe responder 200 con la lista
curl -H "X-Api-Key: AGENDA-LAB8-2026-CLAVE-SUPER-SECRETA" http://localhost:5260/api/Contactos
```

### Probar con Postman

| Configuración | Valor |
|---|---|
| Método | `GET` |
| URL | `http://localhost:5260/api/Contactos` |
| Header | `X-Api-Key`: `AGENDA-LAB8-2026-CLAVE-SUPER-SECRETA` |

Para agregar un contacto:

| Configuración | Valor |
|---|---|
| Método | `POST` |
| URL | `http://localhost:5260/api/Contactos` |
| Headers | `Content-Type`: `application/json` · `X-Api-Key`: `AGENDA-LAB8-2026-CLAVE-SUPER-SECRETA` |
| Body (raw JSON) | `{ "nombre": "Juan Pérez", "numeroTelefonico": "8888-9999" }` |

---

## 6. Levantar el frontend Angular

> El repo del frontend está en https://github.com/1KATZUNO/Laboratorio8_Angular

```powershell
git clone https://github.com/1KATZUNO/Laboratorio8_Angular.git
cd Laboratorio8_Angular\Agenda.Angular
npm install
npm start
```

Cuando termine de compilar:

```
➜  Local:   http://127.0.0.1:49379/
```

Abre esa URL y verás la tabla con los contactos.

### Si la URL o el puerto del API son distintos

Edita `Agenda.Angular/src/environments/environment.ts`:

```ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5260',                   // ← cambia este si tu API usa otro puerto
  apiKey: 'AGENDA-LAB8-2026-CLAVE-SUPER-SECRETA'      // ← debe coincidir con appsettings.json
};
```

Y agrega tu origen al `CORS:OrigenesPermitidos` del API si vas a usar un puerto diferente al `49379` o `4200`.

---

## 7. Flujo completo de prueba

1. ✅ Ejecuta `script_sql\agenda.sql` → BD y 5 contactos creados.
2. ✅ `dotnet run` → API escuchando en `:5260`.
3. ✅ Postman `POST /api/Contactos` con `X-Api-Key` → contacto extra agregado.
4. ✅ `npm start` en Angular → SPA en `:49379`.
5. ✅ Abre el browser → ves la tabla con los contactos (los 5 del seed + el agregado por Postman).

---

## 8. Resolución de problemas comunes

| Síntoma | Causa probable | Solución |
|---|---|---|
| `401 Falta el encabezado X-Api-Key` | No se envió el header `X-Api-Key` | Agregar el header en el cliente |
| `401 API Key inválida` | El valor del header no coincide con `ApiKey:Key` | Sincronizar `appsettings.json` y `environment.ts` |
| Angular muestra error CORS en consola | El origen del frontend no está permitido | Agregar tu URL a `Cors:OrigenesPermitidos` en `appsettings.json` |
| `Cannot open server '...' requested by the login` | Instancia de SQL Server incorrecta | Editar `DefaultConnection` con la cadena correcta |
| `An exception has been raised that is likely due to a transient failure` | No hay conexión o BD no existe | Verificar que el servicio de SQL Server esté corriendo y ejecutar `agenda.sql` |
| Browser dice `ERR_CONNECTION_REFUSED` al puerto de la API | La API no está corriendo o quedó en otro puerto | Revisar `Properties/launchSettings.json` y actualizar `apiUrl` en Angular |
