using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{

    public class OperacionVueloOTDRequest
    {
        public string OpcionOperacion { get; set; }
        public string Id { get; set; }
        public string Fecha { get; set; }
        public string Matricula { get; set; }
        public string Vuelo { get; set; }
        public string Hora { get; set; }
        public string TotalEmbarcados { get; set; }
        public string INF { get; set; }
        public string TTL { get; set; }
        public string TTC { get; set; }
        public string EX { get; set; }
        public string TRIP { get; set; }
        public string PAX { get; set; }
        public string PagoCOP { get; set; }
        public string PagoUSD { get; set; }
        public string ConfirmacionPasajeros { get; set; }
        public string ConfirmacionTransitos { get; set; }
        public string ConfirmacionGenDec { get; set; }
        public string ConfirmacionManifiesto { get; set; }
        public string ConfirmacionOperacion { get; set; }
        public string NombreAerolinea { get; set; }
        public string Destino { get; set; }
        public string Tipo { get; set; }
        public string IdAerolinea { get; set; }
        public string ArchivoGendec { get; set; }
        public string ArchivoManifiesto { get; set; }
        public string ArchivoPasajeros { get; set; }
        public string ArchivoTransito { get; set; }
        public string PDFPasajeros { get; set; }
        public string EstadoProceso { get; set; }
        public string ConsecutivoCargue { get; set; }
        public string IdCargue { get; set; }
        public string IdConsecutivoCargue { get; set; }
        public string NovedadCargue { get; set; }
        public string NovedadProceso { get; set; }
        public string IdVuelo { get; set; }
        public string Id_Daily { get; set; }
        public string tasasReportadas { get; set; }
        public string TotalEmbarcados_LIQ { get; set; }
        public string INF_LIQ { get; set; }
        public string TTL_LIQ { get; set; }
        public string TTC_LIQ { get; set; }
        public string EX_LIQ { get; set; }
        public string TRIP_LIQ { get; set; }
        public string PAX_LIQ { get; set; }
        public string PAGOCOP_LIQ { get; set; }
        public string PAGOUSD_LIQ { get; set; }
        public string EnvioNotificacion { get; set; }
        public string SiglaAerolinea { get; set; }
        public string FechaCreacion { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public OperacionVueloOTDRequest()
        {
            OpcionOperacion = "F";
            Id = "";
            Fecha = "";
            Matricula = "";
            Vuelo = "";
            Hora = "";
            TotalEmbarcados = "";
            INF = "";
            TTL = "";
            TTC = "";
            EX = "";
            TRIP = "";
            PAX = "";
            PagoCOP = "";
            PagoUSD = "";
            ConfirmacionPasajeros = "";
            ConfirmacionTransitos = "";
            ConfirmacionGenDec = "";
            ConfirmacionManifiesto = "";
            ConfirmacionOperacion = "";
            NombreAerolinea = "";
            Destino = "";
            Tipo = "";
            IdAerolinea = "";
            ArchivoGendec = "";
            ArchivoManifiesto = "";
            ArchivoPasajeros = "";
            ArchivoTransito = "";
            PDFPasajeros = "";
            EstadoProceso = "";
            ConsecutivoCargue = "";
            IdCargue = "";
            IdConsecutivoCargue = "";
            NovedadCargue = "";
            NovedadProceso = "";
            IdVuelo = "";
            Id_Daily = "";
            tasasReportadas = "";
            TotalEmbarcados_LIQ = "";
            INF_LIQ = "";
            TTL_LIQ = "";
            TTC_LIQ = "";
            EX_LIQ = "";
            TRIP_LIQ = "";
            PAX_LIQ = "";
            PAGOCOP_LIQ = "";
            PAGOUSD_LIQ = "";
            EnvioNotificacion = "";
            SiglaAerolinea = "";
            FechaCreacion = "";
            FechaDesde = "";
            FechaHasta = "";
        }
    }
}
