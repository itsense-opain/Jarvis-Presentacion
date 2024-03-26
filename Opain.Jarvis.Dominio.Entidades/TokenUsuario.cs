using Microsoft.AspNetCore.Identity;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class TokenUsuario : IdentityUserToken<string>
    {
        public virtual Usuario Usuario { get; set; }
    }
}
