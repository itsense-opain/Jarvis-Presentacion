using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Ticket
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        [Required]
        [ForeignKey("Aerolinea")]
        public int IdAerolinea { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public string IdUsuario { get; set; }

        [Required]
        public int Seguimiento { get; set; }

        public Aerolinea Aerolinea { get; set; }
                
        public Usuario Usuario { get; set; }
       
        public virtual ICollection<RespuestaTicket> RespuestasTickets { get; set; }
    }
}
