using System.Collections.Generic;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class VuelosOtd
    {
        public int Id { get; set; }
        public string NumeroVuelo { get; set; }
        public int IdAerolinea { get; set; }
        public AerolineaOtd Aerolinea { get; set; }
        public string IdOrigen { get; set; }
        public CiudadOtd Origen { get; set; }
        public string IdDestino { get; set; }
        public CiudadOtd Destino { get; set; }
        public bool IdEstado { get; set; }
        public string TipoVuelo { get; set; }
        public IList<OperacionVueloOtd> OperacionesVuelos { get; set; }
        public IList<OperacionVueloOtd> VuelosLlegada { get; set; }
        public IList<OperacionVueloOtd> VuelosSalida { get; set; }
    }
}