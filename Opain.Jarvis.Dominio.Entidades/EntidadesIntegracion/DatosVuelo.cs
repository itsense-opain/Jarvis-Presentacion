using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class DatosVuelo
    {
        public string ID_DAILY { get; set; }
        public string ID_PASAJERO { get; set; }
        public string FECHA_LLEGADA { get; set; }
        public string MATRICULA { get; set; }

       
        public string HORA_LLEGADA { get; set; }
        public string ORIGEN { get; set; }
        public string ORIGEN_DES { get; set; }
        public string ID_AEROLINEA_LLEGADA { get; set; }
        public string AEROLINEA { get; set; }
        public string FECHA_SALIDA { get; set; }
        public string HORA_SALIDA { get; set; }
        public string NUM_VUELO_LLEGANDO { get; set; }
        public string NUM_VUELO_SALIENDO { get; set; }
        public string DESTINO { get; set; }
        public string DESTINODESC { get; set; }
        public string ID_VUELO { get; set; }
        public string TIPO_VUELO { get; set; }

        public string NATURALEZA { get; set; }
    }
}
