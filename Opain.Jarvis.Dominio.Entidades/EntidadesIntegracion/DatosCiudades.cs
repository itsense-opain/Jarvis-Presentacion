using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class DatosCiudades
    {
        public string COD_IATA { get; set; }
        public string COD_OACI { get; set; }
        public string CIUDAD { get; set; }
        public string PAIS { get; set; }
        public string AEROPUERTO { get; set; }

        public int id { get; set; }
    }
}
