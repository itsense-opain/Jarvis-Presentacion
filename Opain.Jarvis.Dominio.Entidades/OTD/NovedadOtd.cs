using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class NovedadOtd
    {
        public int Id { get; set; }
        public int Operacion { get; set; }
        [Display(Name = "Tipo de vuelo")]
        public string TipoVuelo { get; set; }
        [Display(Name = "Número de vuelo")]
        public string NumeroVuelo { get; set; }
        [Display(Name = "Fecha del vuelo")]
        public DateTime FechaVuelo { get; set; }
        [Display(Name = "Hora del vuelo")]
        public string HoraVuelo { get; set; }
        [Display(Name = "Número de matrícula")]
        public string NumeroMatricula { get; set; }
        [Display(Name = "Tipo de novedad")]
        public string TipoNovedad { get; set; }
        public int IdCausal { get; set; }
        [Display(Name = "Código")]
        public string CodCausal { get; set; }
        [Display(Name = "Descripción de la Causal")]
        public string DescCausal { get; set; }
        [Display(Name = "Descripción de la Novedad")]
        public string DescNovedad { get; set; }
        [Display(Name = "Fecha de Novedad")]
        public DateTime FechaHora { get; set; }
        public int IdRegistro { get; set; }
        [Display(Name = "Tipo de Error")]
        public string TipoError { get; set; }
        [Display(Name = "Error Reportado")]
        public string Error { get; set; }
        [Display(Name = "Valores Ingresados")]
        public string Valores { get; set; }
        [Display(Name = "Valores Nuevos Ingresados")]
        public string ValoresNuevos { get; set; }
        public int TipoValidacion { get; set; }
    }
}
