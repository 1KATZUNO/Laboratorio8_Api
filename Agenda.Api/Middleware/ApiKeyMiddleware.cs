namespace Agenda.Api.Middleware
{
    /// <summary>
    /// Middleware personalizado que valida la API Key.
    /// El cliente debe enviar el encabezado HTTP "X-Api-Key" con el valor
    /// configurado en appsettings.json (sección ApiKey:Key).
    ///
    /// Si la clave falta o no coincide, se responde 401 Unauthorized.
    /// </summary>
    public class ApiKeyMiddleware
    {
        public const string NombreHeader = "X-Api-Key";
        private const string SeccionConfig = "ApiKey:Key";

        private readonly RequestDelegate _siguiente;
        private readonly string _apiKeyEsperada;
        private readonly ILogger<ApiKeyMiddleware> _logger;

        public ApiKeyMiddleware(RequestDelegate siguiente, IConfiguration configuracion, ILogger<ApiKeyMiddleware> logger)
        {
            _siguiente = siguiente;
            _logger = logger;
            _apiKeyEsperada = configuracion[SeccionConfig]
                ?? throw new InvalidOperationException(
                    $"Falta la API Key en la configuración. Definir '{SeccionConfig}' en appsettings.json.");
        }

        public async Task InvokeAsync(HttpContext contexto)
        {
            // Permitir CORS preflight sin API Key (el navegador no envía
            // encabezados personalizados en OPTIONS).
            if (HttpMethods.IsOptions(contexto.Request.Method))
            {
                await _siguiente(contexto);
                return;
            }

            if (!contexto.Request.Headers.TryGetValue(NombreHeader, out var apiKeyRecibida))
            {
                _logger.LogWarning("Solicitud sin API Key a {Ruta}", contexto.Request.Path);
                await EscribirNoAutorizadoAsync(contexto, "Falta el encabezado X-Api-Key.");
                return;
            }

            if (!string.Equals(apiKeyRecibida, _apiKeyEsperada, StringComparison.Ordinal))
            {
                _logger.LogWarning("API Key inválida en {Ruta}", contexto.Request.Path);
                await EscribirNoAutorizadoAsync(contexto, "API Key inválida.");
                return;
            }

            await _siguiente(contexto);
        }

        private static Task EscribirNoAutorizadoAsync(HttpContext contexto, string mensaje)
        {
            contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
            contexto.Response.ContentType = "application/json";
            return contexto.Response.WriteAsJsonAsync(new { error = mensaje });
        }
    }

    public static class ApiKeyMiddlewareExtensions
    {
        /// <summary>
        /// Registra el middleware de API Key en el pipeline.
        /// </summary>
        public static IApplicationBuilder UseApiKey(this IApplicationBuilder app)
            => app.UseMiddleware<ApiKeyMiddleware>();
    }
}
