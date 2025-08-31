using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ExperienciasProyecto.Models
{
    public class Compra
    {
        [Key]
        public int ID_COMPRA { get; set; }

        [Required]
        [JsonProperty("ID_USUARIO")]
        public int ID_USUARIO { get; set; }

        [ForeignKey("ID_USUARIO")]
        public virtual Usuario Usuario { get; set; }

        public DateTime FECHA { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        [JsonProperty("TOTAL")]
        public decimal TOTAL { get; set; }

        [JsonProperty("DetallesCompra")]
        public virtual ICollection<DetalleCompra> DetallesCompra { get; set; }
    }
}
