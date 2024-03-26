using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class HorarioAerolineaOtd
    {
        public int Id { get; set; }
        public int IdAerolinea { get; set; }
        public string Aerolinea { get; set; }
        public DateTime Fecha { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
    }
}
