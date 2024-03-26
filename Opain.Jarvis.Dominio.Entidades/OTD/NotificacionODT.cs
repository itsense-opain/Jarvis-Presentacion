using System;
using System.Runtime.Serialization;

namespace Opain.Jarvis.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class NotificacionODT
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string titulo { get; set; }
        [DataMember]
        public string descripcion { get; set; }
        [DataMember]
        public string fecha { get; set; }
        [DataMember]
        public string id_item { get; set; }
        [DataMember]
        public string id_responsable { get; set; }
        [DataMember]
        public string rol_responsable { get; set; }
        [DataMember]
        public string id_aerolinea { get; set; }
        [DataMember]
        public string leido { get; set; }
    }
}
