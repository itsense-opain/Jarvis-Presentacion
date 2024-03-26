using Microsoft.AspNetCore.Identity;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class RolesUsuarios : IdentityUserRole<string>
    {
        public virtual Usuario Usuario { get; set; }
        public virtual Rol Rol { get; set; }
    }

}
