using System.ComponentModel.DataAnnotations;

namespace PuntoDeVentaBin.Shared
{
    public class Usuario
    {
        [Required]
        public string Nombre { get; set; }
    }
}
