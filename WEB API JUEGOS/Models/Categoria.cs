using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExperienciasProyecto.Models
{
    public class Categoria
    {
        public int ID_CATEGORIA { get; set; }
        public string NOMBRE { get; set; }

        public virtual ICollection<Juego> Juegos { get; set; }
    }
}