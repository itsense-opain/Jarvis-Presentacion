using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class CargueOtd
    {
        public int Id { get; set; }

        public string Aerolinea { get; set; }

        public string TipoArchivo { get; set; }

        public DateTime Fecha { get; set; }

        public string Usuario { get; set; }

        public string NombreArchivo { get; set; }

        public string NombreCompletoUsuario { get; set; }

    }
}
