using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExperienciasProyecto.Models
{
	public class HistorialCompra
	{
        public int ID_COMPRA { get; set; }
        public DateTime FECHA { get; set; }
        public string NOMBRE_JUEGO { get; set; }
        public decimal PRECIO_UNITARIO { get; set; }
        public string IMAGEN_URL { get; set; }

    }
}