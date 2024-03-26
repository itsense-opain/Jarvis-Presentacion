using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class DatosAuditoria
    {
        public string Id_Pasajero { get; set; }
        public int Genera_Cobro  { get; set; }
        public int Linea { get; set; }
        public string Campo_Modificado { get; set; }
        public string Valor_Anterior { get; set; }
        public string Valor_Nuevo { get; set; }

    }
}
