using Microsoft.AspNetCore.Identity;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class ClaimRol : IdentityRoleClaim<string>
    {
        public virtual Rol Rol { get; set; }
    }

}
