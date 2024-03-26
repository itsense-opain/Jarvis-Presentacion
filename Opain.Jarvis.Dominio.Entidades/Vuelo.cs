using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Vuelo
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string NumeroVuelo { get; set; }

        [Required]
        [ForeignKey("Aerolinea")]
        public int IdAerolinea { get; set; }
        public Aerolinea Aerolinea { get; set; }

        [Required]
        public string IdOrigen { get; set; }
        public Ciudad Origen { get; set; }

        [Required]
        public string IdDestino { get; set; }
        public Ciudad Destino { get; set; }

        [Required]
        public bool IdEstado { get; set; }

        [Required]
        public string TipoVuelo { get; set; }
        
        public int IdVuelo { get; set; }
        

    }
}
