using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class DatosTransito
    {
        public string Id_Pasajero { get; set; }
        public int Id_Linea { get; set; }
        public string Id_Vuelo { get; set; }
        public string Numero_Vuelo { get; set; }
        public string Tipo_Vuelo { get; set; }
        public DateTime Fecha_Llegada { get; set; }
        public string Hora_Llegada { get; set; }
        public string Aerolinea { get; set; }
        public string Tipo_Transito { get; set; }
        public int Total_Pasajeros { get; set; }
        public string id_aeropuerto { get; set; }
        public string IATACODE { get; set; }
        public string OACICODE { get; set; }
        public string descripcion { get; set; }
        public string descripcion2 { get; set; }
    }
}
