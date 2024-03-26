using Microsoft.AspNetCore.Identity;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class ClaimUsuario : IdentityUserClaim<string>
    {
        public virtual Usuario Usuario { get; set; }
    }

}
