using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ExperienciasProyecto.Models
{
    public class DetalleCompra
    {
        [Key]
        public int ID_DETALLE { get; set; }

        [JsonProperty("ID_COMPRA")]
        public int ID_COMPRA { get; set; }

        [ForeignKey("ID_COMPRA")]
        public virtual Compra Compra { get; set; }

        [JsonProperty("ID_JUEGO")]
        public int ID_JUEGO { get; set; }

        [ForeignKey("ID_JUEGO")]
        public virtual Juego Juego { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [JsonProperty("PRECIO_UNITARIO")]
        public decimal PRECIO_UNITARIO { get; set; }
    }
}
