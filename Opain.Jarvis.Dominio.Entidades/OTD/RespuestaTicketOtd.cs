using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class RespuestaTicketOtd
    {
        public int Id { get; set; }

        public int IdTicket { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public string Adjunto { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        public string NombreUsuario { get; set; }

        public string IdUsuario { get; set; }
    }
}
