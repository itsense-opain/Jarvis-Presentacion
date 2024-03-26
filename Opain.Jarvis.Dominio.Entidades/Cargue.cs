using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Cargue
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        [MaxLength(50)]
        public string Usuario { get; set; }

        [Required]
        public int Tipo { get; set; }

        public string Archivo { get; set; }

        public ICollection<OperacionesVuelo> OperacionesVuelos { get; set; }
    }
}
