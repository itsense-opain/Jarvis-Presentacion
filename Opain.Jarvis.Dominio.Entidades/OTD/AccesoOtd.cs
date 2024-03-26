using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class AccesoOtd
    {
        public int Id { get; set; }
        public string Grupo { get; set; }
        public string Rol { get; set; }
        public string NombreUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }       
        public int IdAeroLineas { get; set; }
    }
}
