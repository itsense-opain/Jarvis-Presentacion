using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class ConsultaOtd
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name="Fecha")]
        public DateTime Fecha { get; set; }
        public int Id { get; set; }
        [Display(Name = "Cantidad de vuelos")]
        public int CantidadVuelos { get; set; }
        [Display(Name = "Novedades de cargue")]
        public int NovedadesCargue { get; set; }
        [Display(Name = "Novedades de proceso")]
        public int NovedadesProceso { get; set; }
        [Display(Name = "Exitoso")]
        public int Exitoso { get; set; }
        [Display(Name = "Procesados")]
        public int Procesados { get; set; }
        [Display(Name = "Finalizado")]
        public int Finalizados { get; set; }
    }
}
