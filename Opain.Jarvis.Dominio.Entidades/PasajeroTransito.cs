using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class PasajeroTransito
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
        public DateTime FechaLlegada { get; set; }

        [Required]
        public string HoraLlegada { get; set; }

        [Required]
        public int IdVueloLlegada { get; set; }

        [Required]
        public string NumeroVueloLlegada { get; set; }

        [Required]
        public string Origen { get; set; }

        [Required]
        public DateTime FechaSalida { get; set; }

        [Required]
        public string HoraSalida { get; set; }

        [Required]
        public int IdVueloSalida { get; set; }

        [Required]
        public string NumeroVueloSalida { get; set; }

        [Required]
        public string Destino { get; set; }

        [Required]
        [MaxLength(250)]
        public string NombrePasajero { get; set; }

        [Required]
        [MaxLength(5)]
        public string Categoria { get; set; }

        public DateTime FechaHoraCargue { get; set; }

        public DateTime FechaHoraFirma { get; set; }

        public int Firmado { get; set; }
         
        [Required]
        public string AerolineaLlegada { get; set; }
        [Required]
        public string AerolineaSalida { get; set; }
        
        public int IdCargue { get; set; }
        
    }
}
