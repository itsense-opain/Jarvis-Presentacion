using System;
using System.ComponentModel.DataAnnotations;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class PasajeroTransitoOtd
    {
        public int Id { get; set; }

        public int Operacion { get; set; }

        [Display(Name = "Fecha de Llegada")]
        [DataType(DataType.Date)]
       // [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaLlegada { get; set; }

        [Display(Name = "Hora de Llegada")]
        [DataType(DataType.Time)]
        public string HoraLlegada { get; set; }

        [Display(Name = "Vuelo de Llegada")]
        public string NumeroVueloLlegada { get; set; }

        [Display(Name = "Origen")]
        public string Origen { get; set; }

        [Display(Name = "Fecha de Salida")]        
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaSalida { get; set; }

        [Display(Name = "Hora de Salida")]
        [DataType(DataType.Time)]
        public string HoraSalida { get; set; }

        [Display(Name = "Vuelo de salida")]
        public string NumeroVueloSalida { get; set; }

        [Display(Name = "Destino")]
        public string Destino { get; set; }

        [Display(Name = "Nombre del pasajero")]
        public string NombrePasajero { get; set; }

        [Display(Name = "Categoría")]
        public int TTC { get; set; }

        [Display(Name = "Categoria")]
        public int TTL { get; set; }

        [Display(Name = "Fecha y Hora de Cargue")] 
        public DateTime FechaHoraCargue { get; set; }

        [Display(Name = "Fecha y Hora de Firmado")]
        public DateTime FechaHoraFirma { get; set; }

        [Display(Name = "Firmado")]
        public int Firmado { get; set; }

        [Display(Name = "Aerolínea de Salida")]
        public string AerolineaSalida { get; set; }
                      
        [Display(Name = "Aerolínea de Llegada")]
        public string AerolineaLlegada { get; set; }

        public string Tipo { get; set; }

        public string NombreAerolinea { get; set; }
        public string TipoVuelo { get; set; }
        public string Observaciones { get; set; }

        public int IdCargue { get; set; }

    }
}
