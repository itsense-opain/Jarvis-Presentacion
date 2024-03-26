using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class NovedadProceso
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("OperacionesVuelo")]
        public int IdOperacionVuelo { get; set; }
        public OperacionesVuelo OperacionesVuelo { get; set; }

        [Required]
        public int TipoNovedad { get; set; }

        [Required]
        [ForeignKey("Causal")]
        public int IdCausal { get; set; }
        public Causal Causal { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public DateTime FechaHora { get; set; }

        public int IdRegistro { get; set; }
    }
}
