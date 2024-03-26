using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class RespuestaValidacionManual
    {
        public int CantPasajeros { get; set; }
        public int CantTransitos { get; set; }

        public int CantInfantes { get; set; }

        public int CantTTL { get; set; }
        public int CantTTC { get; set; }
        public int CantEX { get; set; }
        public int CantTRIP { get; set; }
        public decimal TasaCOP { get; set; }
        public decimal TasaUSD { get; set; }

    }
}
