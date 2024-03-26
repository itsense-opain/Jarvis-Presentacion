using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class Acceso
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(45)]
        public string Grupo { get; set; }

        [Required]
        [MaxLength(45)]
        public string Rol { get; set; }

        [Required]
        [MaxLength(150)]
        public string NombreUsuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [MaxLength(5)]
        public string Hora { get; set; }

        [Required]
        [MaxLength(11)]
        public int IdAeroLineas { get; set; }

    }
}
