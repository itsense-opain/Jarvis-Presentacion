using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class CausalOtd
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Cod. de Causal")]
        public string Codigo { get; set; }


        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Tipo")]
        public int Tipo { get; set; }

        [Display(Name = "Estado")]
        public int Estado { get; set; }
    }
}
