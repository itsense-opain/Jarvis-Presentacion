using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{

    public class HorarioOperacionOtd
    {
        public int Id { get; set; }

        [Display(Name ="Día")]
        public string Dia { get; set; }

        [Display(Name = "Hora de inicio")]        
        public string HoraInicio { get; set; }

        [Display(Name = "Hora final")]
        public string HoraFin { get; set; }
    }
}
