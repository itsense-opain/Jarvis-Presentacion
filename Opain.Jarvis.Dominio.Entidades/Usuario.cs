using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Usuario : IdentityUser
    {
        public Usuario() : base() { }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cargo { get; set; }
        public string Telefono { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        [Required]
        public bool Activo { get; set; }

        public virtual ICollection<ClaimUsuario> ClaimUsuario { get; set; }
        public virtual ICollection<LoginUsuario> LoginUsuario { get; set; }
        public virtual ICollection<TokenUsuario> TokenUsuario { get; set; }
        public virtual ICollection<RolesUsuarios> RolesUsuarios { get; set; }
        public virtual ICollection<UsuariosAerolineas> UsuariosAerolineas { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<RespuestaTicket> RespuestasTickets { get; set; }
    }
}
