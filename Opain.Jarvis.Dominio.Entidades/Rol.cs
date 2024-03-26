using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Rol : IdentityRole
    {
        public Rol() : base() { }

        public Rol(string roleName)
            : base(roleName)
        {
            base.Name = roleName;
        }

        public virtual ICollection<RolesUsuarios> RolesUsuarios { get; set; }
        public virtual ICollection<ClaimRol> ClaimRol { get; set; }
    }

}
