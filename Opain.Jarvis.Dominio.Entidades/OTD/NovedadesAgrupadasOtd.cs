using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class NovedadesAgrupadasOtd
    {
        public DateTime FechaVuelo { get; set; }
        public int CantidadVuelos { get; set; }
        public int NovedadesCargue { get; set; }
        public int NovedadesProcesos { get; set; }
        public bool Exitoso { get; set; }
        public int Procesados { get; set; }
    }
}
