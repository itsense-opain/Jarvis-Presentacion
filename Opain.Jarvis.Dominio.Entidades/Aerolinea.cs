using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Aerolinea
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; }

        [Required]
        public bool IdEstado { get; set; }

        [Required]
        public int PDFPasajeros { get; set; }

        public string Codigo { get; set; }

        public string Sigla { get; set; }

        [Required]
        public int CantidadUsuarios { get; set; }

        public IList<Vuelo> Vuelos { get; set; }
        public IList<UsuariosAerolineas> UsuariosAerolineas { get; set; }

        public IList<HorarioAerolinea> HorarioAerolinea { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

        public IList<OperacionesVuelo> OperacionesVuelos { get; set; }
    }
}
