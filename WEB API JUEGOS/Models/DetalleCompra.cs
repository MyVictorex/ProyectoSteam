using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExperienciasProyecto.Models
{
	public class DetalleCompra
	{
        [Key]
        public int ID_DETALLE { get; set; }

        public int ID_COMPRA { get; set; }

        [ForeignKey("ID_COMPRA")]
        public virtual Compra Compra { get; set; }

        public int ID_JUEGO { get; set; }

        [ForeignKey("ID_JUEGO")]
        public virtual Juego Juego { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PRECIO_UNITARIO { get; set; }
    }
}