using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Notificacion
    {
        public string Origen { get; set; }       
        public string Destinos { get; set; }
        public string Cuerpo { get; set; }
        public short Tipo { get; set; }
        public string Copias { get; set; }
        public string Asunto { get; set; }
    }
}
