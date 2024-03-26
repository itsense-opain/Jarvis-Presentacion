using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class CargueArchivo
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(45)]
        public string Aerolinea { get; set; }

        [Required]
        [MaxLength(45)]
        public string TipoArchivo { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        //[Required]
        public string Usuario { get; set; }


        //[Required]
        public string NombreArchivo { get; set; }


        //[Required]
        //public string NombreCompletoUsuario { get; set; }




    }
}
