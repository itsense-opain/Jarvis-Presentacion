using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class TicketOtd
    {
        public int Id { get; set; }

        [Required]
        public string NumeroTicket { get; set; }

        [Required]
        public int TipoConsulta { get; set; }

        [Required]
        public string Asunto { get; set; }

        public DateTime FechaVuelo { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public string Adjunto { get; set; }

        [Required]
        public int Estado { get; set; }
                
        public string NombreAerolinea { get; set; }

        public int IdAerolinea { get; set; }

        [Required]
        public string NombreUsuario { get; set; }

        public string IdUsuario { get; set; }

        [Required]
        public int Seguimiento { get; set; }

        public ICollection<RespuestaTicketOtd> Respuestas { get; set; }
    }
}
