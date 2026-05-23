using System.ComponentModel.DataAnnotations;

namespace Agenda.Api.Models
{
    /// <summary>
    /// Contacto de la agenda telefónica.
    /// Refleja la tabla Contactos en la base de datos.
    /// </summary>
    public class Contacto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número telefónico es obligatorio.")]
        [StringLength(30, ErrorMessage = "El número telefónico no puede superar los 30 caracteres.")]
        public string NumeroTelefonico { get; set; } = string.Empty;
    }
}
