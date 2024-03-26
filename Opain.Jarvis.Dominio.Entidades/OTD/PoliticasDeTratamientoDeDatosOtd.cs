using System;
using System.Collections.Generic;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class PoliticasDeTratamientoDeDatosOtd
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public bool AceptarPoliticas { get; set; }       
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }

        //nuevos campos solicitados
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string NumeroDocumento { get; set; }
        public string Cargo { get; set; }
        public string Aerolinea { get; set; }
    }
}
