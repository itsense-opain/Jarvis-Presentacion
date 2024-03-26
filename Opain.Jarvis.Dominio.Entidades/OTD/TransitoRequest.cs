using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
     public class TransitoRequest
    {
        public string Id { get; set; }
        public string Operacion { get; set; }
        public string FechaLlegada { get; set; }
        public string HoraLlegada { get; set; }
        public string NumeroVueloLlegada { get; set; }
        public string Origen { get; set; }
        public string FechaSalida { get; set; }
        public string HoraSalida { get; set; }
        public string NumeroVueloSalida { get; set; }
        public string Destino { get; set; }
        public string NombrePasajero { get; set; }
        public string TTC { get; set; }
        public string TTL { get; set; }
        public string FechaHoraCargue { get; set; }
        public string FechaHoraFirma { get; set; }
        public string Firmado { get; set; }
        public string AerolineaSalida { get; set; }
        public string AerolineaLlegada { get; set; }
        public string Tipo { get; set; }
        public string NombreAerolinea { get; set; }
        public string TipoVuelo { get; set; }
        public string Observaciones { get; set; }
        public string IdCargue { get; set; }
        public string OpcionOperacion { get; set; }
        public string EstadoProceso { get; set; }
        public string Categoria { get; set; }
        public TransitoRequest()
        {
            OpcionOperacion = "F";
            Id = "";
            Operacion = "";
            FechaLlegada = "";
            HoraLlegada = "";
            NumeroVueloLlegada = "";
            Origen = "";
            FechaSalida = "";
            HoraSalida = "";
            NumeroVueloSalida = "";
            Destino = "";
            NombrePasajero = "";
            TTC = "";
            TTL = "";
            FechaHoraCargue = "";
            FechaHoraFirma = "";
            Firmado = "";
            AerolineaSalida = "";
            AerolineaLlegada = "";
            Tipo = "";
            NombreAerolinea = "";
            TipoVuelo = "";
            Observaciones = "";
            IdCargue = "";
            Categoria = "";
        }
    }
}
