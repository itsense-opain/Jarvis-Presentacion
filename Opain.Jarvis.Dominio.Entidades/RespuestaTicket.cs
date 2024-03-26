using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class RespuestaTicket
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Ticket")]
        public int IdTicket { get; set; }
        public Ticket Ticket { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public string Adjunto { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public string IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
    }
}
