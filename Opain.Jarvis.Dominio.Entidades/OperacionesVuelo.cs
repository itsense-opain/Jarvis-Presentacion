using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class OperacionesVuelo
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdVuelo { get; set; }
        public string Id_Daily { get; set; }


        [Required]
        [ForeignKey("Aerolinea")]
        public int IdAerolinea { get; set; }
        public Aerolinea Aerolinea { get; set; }

        [Required]
        public string TipoVuelo { get; set; }

        [Required]
        public string NumeroVuelo { get; set; }

        [Required]
        public string Destino { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime FechaVuelo { get; set; }

        [Required]
        [MaxLength(15)]
        public string MatriculaVuelo { get; set; }

        [Required]
        [MaxLength(5)]
        public string HoraVuelo { get; set; }

        [Required]
        public int TotalEmbarcados { get; set; }

        [Required]
        public int ConfirmacionPasajeros { get; set; }

        [Required]
        public int ConfirmacionTransitos { get; set; }

        [Required]
        public int ConfirmacionGenDec { get; set; }

        [Required]
        public int CanfirmacionManifiesto { get; set; }

        [Required]
        public int ConfirmacionOperacion { get; set; }

        [Required]
        public int INF { get; set; }

        [Required]
        public int TTL { get; set; }

        [Required]
        public int TTC { get; set; }

        [Required]
        public int EX { get; set; }

        [Required]
        public int TRIP { get; set; }

        [Required]
        public int PAX { get; set; }

        [Required]
        public int PagoCOP { get; set; }

        [Required]
        public int PagoUSD { get; set; }

        [Required]
        public int EstadoProceso { get; set; }

        [Required]
        [ForeignKey("Cargue")]
        public int IdCargue { get; set; }
        public Cargue Cargue { get; set; }

        public int? tasasReportadas { get; set; }

        public ICollection<Archivo> Archivos { get; set; }
        public ICollection<Pasajero> Pasajeros { get; set; }
        public ICollection<PasajeroTransito> PasajerosTransitos { get; set; }
        public ICollection<NovedadProceso> NovedadesProceso { get; set; }

        public int? TotalEmbarcados_LIQ { get; set; }
        public int? INF_LIQ { get; set; }
        public int? TTL_LIQ { get; set; }
        public int? TTC_LIQ { get; set; }
        public int? EX_LIQ { get; set; }
        public int? TRIP_LIQ { get; set; }
        public int? PAX_LIQ { get; set; }
        public int? PAGOCOP_LIQ { get; set; }
        public int? PAGOUSD_LIQ { get; set; }

        public int EnvioNotificacion { get; set; }

        public DateTime FechaCreacion { get; set; }


    }
}
