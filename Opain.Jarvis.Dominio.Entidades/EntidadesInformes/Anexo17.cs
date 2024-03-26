using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Anexo17 : Base
    {
        public string Factura { get; set; }
        public string NombreAerolinea { get; set; }
        public string IdGPU { get; set; }
        public string Matricula { get; set; }
        public string NumeroVueloIngreso { get; set; }
        public string NumeroVueloSalida { get; set; }
        public string FechaConexion { get; set; }
        public string HoraConexion { get; set; }
        public string FechaDesconexion { get; set; }
        public string HoraDesconexion { get; set; }
        public string Minutos { get; set; }
        public string TarifaUSD { get; set; }
        public string TRM { get; set; }
        public string TarifaCOP { get; set; }
        public string TotalCOP { get; set; }        
    }
}
