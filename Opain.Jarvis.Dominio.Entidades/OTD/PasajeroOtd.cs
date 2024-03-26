using System;
using System.ComponentModel.DataAnnotations;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class PasajeroOtd
    {
        public int Id { get; set; }
        public int Operacion { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [Display(Name ="Vuelo")]
        public string NumeroVuelo { get; set; }
        [Display(Name = "Matrícula")]
        public string MatriculaVuelo { get; set; }
        [Display(Name = "Nombre del pasajero")]
        public string NombrePasajero { get; set; }
        [Display(Name = "Categoría")]
        public string Categoria { get; set; }
        public string NombreAerolinea { get; set; }
        public string TipoVuelo { get; set; }
        public string realiza_viaje { get; set; }
        public string motivo_exencion { get; set; }
        public int Origen { get; set; }
        public int IdCargue { get; set; }


    }
}
