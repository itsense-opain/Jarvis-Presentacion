using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Ciudad
    {
        [Key]
        [Required]
        [MaxLength(3)]
        public string Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; }

        [Required]
        [ForeignKey("Pais")]
        public string IdPais { get; set; }
        public Pais Pais { get; set; }
        public string Codigo { get; set; }
        
        [Required]
        public bool IdEstado { get; set; }

        public ICollection<Vuelo> Origenes { get; set; }

        public ICollection<Vuelo> Destinos { get; set; }
    }
}
