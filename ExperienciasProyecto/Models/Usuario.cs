using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExperienciasProyecto.Models
{
	public class Usuario
	{
        [Key]
        public int ID_USUARIO { get; set; }

        [Required]
        [StringLength(100)]
        public string NOMBRE { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string CORREO { get; set; }

        [Required]
        [StringLength(100)]
        public string CONTRASENA { get; set; }

        [StringLength(50)]
        public string ROL { get; set; }
        // Relaciones
        public virtual ICollection<Compra> Compras { get; set; }
        public virtual ICollection<Recomendacion> Recomendaciones { get; set; }

    }
}