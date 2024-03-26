using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class PoliticasDeTratamientoDeDatos
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string NombreUsuario { get; set; }


        [Required]
        [Column(TypeName = "BIT")]
        public bool AceptarPoliticas { get; set; }


        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [MaxLength(5)]
        public string Hora { get; set; }
        
        //nuevos campos solicitados
        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string NumeroDocumento { get; set; }

        [Required]
        [MaxLength(50)]
        public string Cargo { get; set; }

        [Required]
        [MaxLength(150)]
        public string Aerolinea { get; set; }
        
    }
}
