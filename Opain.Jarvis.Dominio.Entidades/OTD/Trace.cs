using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Trace
    {
        public int IDEvento { get; set; }
        public string Aplicacion { get; set; }
        public string Mensaje { get; set; }
        public int Severidad { get; set; }
        public string Usuario { get; set; }

    }
}
