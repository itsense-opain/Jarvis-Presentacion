using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class HorarioAerolinea
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Aerolinea")]
        public int IdAerolinea { get; set; }
        public Aerolinea Aerolinea { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [MaxLength(5)]
        public string HoraInicio { get; set; }

        [Required]
        [MaxLength(5)]
        public string HoraFin { get; set; }
    }
}
