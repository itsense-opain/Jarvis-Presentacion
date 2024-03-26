using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class VuelosValidados
    {
        public int IdOperacionVuelo { get; set; }
        public int IdVuelo { get; set; }
        public string Matricula { get; set; }
        public string NumeroVuelo { get; set; }
        public DateTime FechaVuelo { get; set; }
        public string HoraVuelo { get; set; }
        public int Infantes { get; set; }
        public int Adultos { get; set; }
        public int Tripulacion { get; set; }
        public int PagoCOP { get; set; }
        public int PagoUSD { get; set; }
        public int TotalLinea { get; set; }
        public int TotalConexion { get; set; }
        public string Aerolinea { get; set; }
        public int TotalEmbarcados { get; set; }
        public string Id_daily { get; set; }
        public string Id_pasajero { get; set; }
        public string TipoVuelo { get; set; }
        public string IDAEROPUERTO { get; set; }
        public string IATACODE { get; set; }
        public string OACICODE { get; set; }

    }
}
