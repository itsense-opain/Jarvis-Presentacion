using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Causales
    {
        public  string Id_Pasajero { get; set; }
        public string Causal { get; set; }
        public int Linea { get; set; }
        public int TotalPax { get; set; }
        public string TipoVuelo { get; set; }
        public int GeneraCobro { get; set; }

    }
}

