using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class InformeCobro
    {
        public DateTime FechaSalida { get; set; }
        public string Matricula { get; set; }
        public string TipoVuelo { get; set; }
        public string Oaci { get; set; }
        public string NumeroVuelo { get; set; }
        public string TasasReportadas { get; set; }
        public string TasasCobradas { get; set; }
        public int Pasajeros { get; set; }
        public int Infantes { get; set; }
        public int Linea { get; set; }
        public int TransitoConexion { get; set; }
        public int exentos { get; set; }
        public int tripulantes { get; set; }
        public string DiferenciaTasas { get; set; }
        public string Concepto { get; set; }
        public string Usuario { get; set; }
        public DateTime fechaCargue { get; set; }
        public string HoraCargue { get; set; }
        public List<string> lstNovedades { get; set; }
    }
}

