using Agenda.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Api.Data
{
    /// <summary>
    /// Contexto EF Core de la Agenda. Una sola entidad: Contacto.
    /// </summary>
    public class AgendaDbContext : DbContext
    {
        public AgendaDbContext(DbContextOptions<AgendaDbContext> options) : base(options)
        {
        }

        public DbSet<Contacto> Contactos => Set<Contacto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contacto>(entidad =>
            {
                entidad.ToTable("Contactos");
                entidad.HasKey(c => c.Id);
                entidad.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
                entidad.Property(c => c.NumeroTelefonico).IsRequired().HasMaxLength(30);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
