using System.Collections.Generic;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class CiudadOtd
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string IdPais { get; set; }
        public string Codigo { get; set; }
        public bool IdEstado { get; set; }
        public IList<VuelosOtd> Origenes { get; set; }
        public IList<VuelosOtd> Destinos { get; set; }
    }
}