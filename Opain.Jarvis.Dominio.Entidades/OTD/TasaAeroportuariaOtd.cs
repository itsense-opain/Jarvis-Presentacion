using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class TasaAeroportuariaOtd
    {

        public int Id { get; set; }

        public float ValorCOP { get; set; }

        public float ValorUSD { get; set; }

        public DateTime Fecha { get; set; }
    }
}
