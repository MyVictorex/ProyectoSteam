using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExperienciasProyecto.Models
{
	public class Recomendacion
	{

        [Key]
        public int ID_RECOMENDACION { get; set; }

        public int ID_USUARIO { get; set; }

        [ForeignKey("ID_USUARIO")]
        public virtual Usuario Usuario { get; set; }

        public int ID_JUEGO { get; set; }

        [ForeignKey("ID_JUEGO")]
        public virtual Juego Juego { get; set; }

        [StringLength(200)]
        public string MOTIVO { get; set; }
    }
}