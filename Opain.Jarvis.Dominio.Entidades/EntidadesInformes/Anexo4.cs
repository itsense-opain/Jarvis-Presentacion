using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Anexo4 : Base
    {
        public string Factura { get; set; }
        public string Aerolinea { get; set; }
        public string FechaVuelo { get; set; }
        public string Matricula { get; set; }
        public string Sigla { get; set; }
        public string NumeroVuelo { get; set; }
        public string TarifaCOP { get; set; }
        public string TarifaUSD { get; set; }
        public string PaganCOP { get; set; }
        public string PaganUSD { get; set; }
        public string CobroCOP { get; set; }
        public string CobroUSD { get; set; }
        public string VrComisionCOP { get; set; }
        public string VrComisionUSD { get; set; }
        public string CodigoVueloSalida { get; set; }
    }
}
