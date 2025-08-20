using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExperienciasProyecto.Models
{
	public class CarritoItem
	{
        public int ID_JUEGO { get; set; }
        public string NOMBRE { get; set; }
        public decimal PRECIO { get; set; }
        public string IMAGEN_URL { get; set; }

    }
}