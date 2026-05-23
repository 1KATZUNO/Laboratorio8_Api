using Agenda.Api.Models;

namespace Agenda.Api.Services
{
    /// <summary>
    /// Contrato de la capa de lógica de negocio.
    /// Aísla al controlador del DbContext.
    /// </summary>
    public interface IContactoService
    {
        Task<IReadOnlyList<Contacto>> ObtenerTodosAsync();
        Task<Contacto?> ObtenerPorIdAsync(int id);
        Task<Contacto> AgregarAsync(Contacto contacto);
    }
}
