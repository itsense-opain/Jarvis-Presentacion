using System.Collections.Generic;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class UsuariosAerolineasOtd
    {
        public int Id { get; set; }
        public int IdAerolinea { get; set; }
        public string IdUsuario { get; set; }

        public AerolineaOtd Aerolinea { get; set; }
    }
}