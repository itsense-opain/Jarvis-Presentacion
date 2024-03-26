using System.ComponentModel.DataAnnotations;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class IngresoOtd
    {
        [Required]
        public string Usuario { get; set; }
        [Required]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

    }
}
