using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class ConsecutivoCargueOtd
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string Usuario { get; set; }
        public string Consecutivo { get; set; }
        public string Tipo { get; set; }
        public string Archivo { get; set; }
    }
}
