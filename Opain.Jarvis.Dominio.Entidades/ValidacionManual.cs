using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class ValidacionManual
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(10)]
        public int IdVuelo { get; set; }

        [StringLength(3)]
        public int CantPasajeros_Old { get; set; }
        [StringLength(3)]
        public int CantPasajeros_New { get; set; }

        [StringLength(3)]
        public string CausalPasajeros { get; set; }
        [StringLength(3)]
        public int CantTransitos_Old { get; set; }
        [StringLength(3)]
        public int CantTransitos_New { get; set; }
        [StringLength(3)]
        public string CausalTransitos { get; set; }
        [StringLength(3)]
        public int CantInfantes_Old { get; set; }
        [StringLength(3)]
        public int CantInfantes_New { get; set; }
        [StringLength(3)]
        public string CausalInfantes { get; set; }
        [StringLength(3)]
        public int CantTTL_Old { get; set; }
        [StringLength(3)]
        public int CantTTL_New { get; set; }
        [StringLength(3)]
        public string CausalTTL { get; set; }
        [StringLength(3)]
        public int CantTTC_Old { get; set; }
        [StringLength(3)]
        public int CantTTC_New { get; set; }
        [StringLength(3)]
        public string CausalTTC { get; set; }
        [StringLength(3)]
        public int CantEX_Old { get; set; }
        [StringLength(3)]
        public int CantEX_New { get; set; }
        [StringLength(3)]
        public int CausalEX { get; set; }
        [StringLength(3)]
        public int CantTRIP_Old { get; set; }
        [StringLength(3)]
        public int CantTRIP_New { get; set; }
        [StringLength(3)]
        public string CausalTRIP { get; set; }
        [StringLength(3)]
        public int CantPagoCOP_Old { get; set; }
        [StringLength(3)]
        public int CantPagoCOP_New { get; set; }
        [StringLength(3)]
        public string CausalPagoCOP { get; set; }
        [StringLength(3)]
        public int CantPagoUSD_Old { get; set; }
        [StringLength(3)]
        public int CantPagoUSD_New { get; set; }
        [StringLength(3)]
        public string CausalPagoUSD { get; set; }

    }
}
