using System.Collections.Generic;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class AerolineaOtd
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string IdEstado { get; set; }
        public string PDFPasajeros { get; set; }
        public string Codigo { get; set; }
        public int CantidadUsuarios { get; set; }

        public string Sigla { get; set; }


        public IList<HorarioAerolineaOtd> HorarioAerolinea { get; set; }
        public IList<VuelosOtd> Vuelos { get; set; }
    }
}