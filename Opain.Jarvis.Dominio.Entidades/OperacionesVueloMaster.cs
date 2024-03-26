using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class OperacionesVueloMaster
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Vuelo")]
        public int IdVuelo { get; set; }
        public Vuelo Vuelo { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public DateTime FechaVuelo { get; set; }
        [Required]
        [MaxLength(5)]
        public string HoraVuelo { get; set; }
        [Required]
        [MaxLength(15)]
        public string MatriculaVuelo { get; set; }

    }
}
