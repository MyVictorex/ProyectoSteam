using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExperienciasProyecto.Models
{
	public class RecomendacionVista
	{
        public int ID_RECOMENDACION { get; set; }
        public string NOMBRE_JUEGO { get; set; }
        public string MOTIVO { get; set; }
        public string IMAGEN_URL { get; set; }

    }
}