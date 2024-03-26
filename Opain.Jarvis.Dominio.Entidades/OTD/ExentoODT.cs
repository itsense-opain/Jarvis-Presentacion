using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class ExentoODT
    {
        public int Id { get; set; }        
        public string id_vuelo { get; set; }
        public string nombre { get; set; }
        public string tipo_exento { get; set; }
        public string realiza_viaje { get; set; }
        public string motivo_exencion { get; set; }

        public DateTime Fecha { get; set; }

        public string Pasajero { get; set; }

        public string Matricula { get; set; }

        public string Terminal { get; set; }

        public string idpax1 { get; set; }

        public string idpax2 { get; set; }

    }
}
