using Agenda.Api.Data;
using Agenda.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Api.Services
{
    public class ContactoService : IContactoService
    {
        private readonly AgendaDbContext _contexto;

        public ContactoService(AgendaDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<IReadOnlyList<Contacto>> ObtenerTodosAsync()
        {
            return await _contexto.Contactos
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Contacto?> ObtenerPorIdAsync(int id)
        {
            return await _contexto.Contactos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Contacto> AgregarAsync(Contacto contacto)
        {
            ArgumentNullException.ThrowIfNull(contacto);
            _contexto.Contactos.Add(contacto);
            await _contexto.SaveChangesAsync();
            return contacto;
        }
    }
}
