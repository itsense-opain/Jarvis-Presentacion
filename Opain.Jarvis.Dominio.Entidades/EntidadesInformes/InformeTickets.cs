using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class InformeTickets
    {
        public string NumeroTicket { get; set; }
        public string TipoTicket { get; set; }
        public string EstadoFinal { get; set; }
        public string Depende { get; set; }
        public string Asunto { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Aerolinea { get; set; }
        public string Usuario { get; set; }
        public string Adjunto { get; set; }
        public string Respuesta { get; set; }
        public DateTime FechaRespuesta { get; set; }
        public string AdjuntoRespuesta { get; set; }
        public string UsuarioRespuesta { get; set; } 
    }
}
