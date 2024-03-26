using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class HorarioOperacion
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [MaxLength(1)]
        public string Dia { get; set; }

        [Required]
        [MaxLength(5)]
        public string HoraInicio { get; set; }

        [Required]
        [MaxLength(5)]
        public string HoraFin { get; set; }
    }
}
