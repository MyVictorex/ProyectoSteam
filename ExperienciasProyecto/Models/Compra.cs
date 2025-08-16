using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExperienciasProyecto.Models
{
	public class Compra
	{
        [Key]
        public int ID_COMPRA { get; set; }

        [Required]
        public int ID_USUARIO { get; set; }

        [ForeignKey("ID_USUARIO")]
        public virtual Usuario Usuario { get; set; }

        public DateTime FECHA { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TOTAL { get; set; }

        public virtual ICollection<DetalleCompra> DetallesCompra { get; set; }
    }
}