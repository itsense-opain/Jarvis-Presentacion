using System.ComponentModel.DataAnnotations;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class RegistroOtd
    {
        [Required]
        public string Usuario { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }
    }
}
