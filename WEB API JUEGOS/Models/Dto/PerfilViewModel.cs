using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExperienciasProyecto.Models;

namespace WEB_API_JUEGOS.Models.Dto
{
    public class PerfilViewModel
    {
        public Usuario Usuario { get; set; }

        public IEnumerable<HistorialCompra> HistorialDeCompras { get; set; }

        public decimal TotalGastado { get; set; }

        public IEnumerable<RecomendacionVista> Recomendaciones { get; set; }
    }
}

