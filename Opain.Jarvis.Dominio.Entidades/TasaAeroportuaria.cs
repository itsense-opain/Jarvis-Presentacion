using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class TasaAeroportuaria
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public decimal ValorCOP { get; set; }

        [Required]
        public decimal ValorUSD { get; set; }

        [Required]
        public DateTime Fecha { get; set; }
    }
}
