using Microsoft.AspNetCore.Identity;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class LoginUsuario : IdentityUserLogin<string>
    {
        public virtual Usuario Usuario { get; set; }
    }

}
