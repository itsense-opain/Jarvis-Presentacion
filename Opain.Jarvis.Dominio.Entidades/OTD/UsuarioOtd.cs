using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class UsuarioOtd
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "No cumple caracteristicas de correo electronico.")]
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public int EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public int PhoneNumberConfirmed { get; set; }
        public int TwoFactorEnabled { get; set; }
        public string LockoutEnd { get; set; }
        public string LockoutEnabled { get; set; }
        public string AccessFailedCount { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Telefono")]
        [Phone]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Tipo de Documento")]
        public string TipoDocumento { get; set; }
        [Display(Name = "Documento de Identidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string NumeroDocumento { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Perfil")]
        public string Perfil { get; set; }
        public string Aerolinea { get; set; }
        public bool Activo { get; set; }
        public string Clave { get; set; }

        public IList<UsuariosAerolineasOtd> UsuarioAerolinea { get; set; }
        public IList<RolesUsuariosOtd> RolesUsuario { get; set; }
    }

    public class Registros
    {
        
        public string IdAerolinea { get; set; }
        public string Id { get; set; }
        public string Tipo { get; set; }
        public string NumeroVuelo { get; set; }
        public string FechaVuelo { get; set; }
        public string HoraVuelo { get; set; }
        public string NumeroMatricula { get; set; }
        public string TasasReportadas { get; set; }
        public string TasasCobradas { get; set; }
        public string DiferenciaDeTasas { get; set; }
        public string EnvioNotificacion { get; set; }
    }
}
