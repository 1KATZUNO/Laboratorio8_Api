using Agenda.Api.Models;
using Agenda.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Api.Controllers
{
    /// <summary>
    /// Controlador REST de contactos. El middleware de API Key valida
    /// el encabezado X-Api-Key antes de llegar aquí, así que cualquier
    /// solicitud que entre a este controlador ya está autorizada.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ContactosController : ControllerBase
    {
        private readonly IContactoService _servicio;

        public ContactosController(IContactoService servicio)
        {
            _servicio = servicio;
        }

        // GET /api/Contactos — usado por el frontend Angular para listar.
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Contacto>>> ObtenerTodos()
        {
            var contactos = await _servicio.ObtenerTodosAsync();
            return Ok(contactos);
        }

        // GET /api/Contactos/{id} — detalle (también consumible desde Postman).
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Contacto>> ObtenerPorId(int id)
        {
            var contacto = await _servicio.ObtenerPorIdAsync(id);
            if (contacto is null)
            {
                return NotFound();
            }
            return Ok(contacto);
        }

        // POST /api/Contactos — únicamente para carga manual desde Postman.
        [HttpPost]
        public async Task<ActionResult<Contacto>> Agregar([FromBody] Contacto contacto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var creado = await _servicio.AgregarAsync(contacto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }
    }
}
