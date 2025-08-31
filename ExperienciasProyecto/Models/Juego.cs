using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExperienciasProyecto.Models
{
    public class Juego
    {
        [Key]
        public int ID_JUEGO { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
        public string NOMBRE { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(500, ErrorMessage = "La descripción no debe exceder los 500 caracteres.")]
        public string DESCRIPCION { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 1000.00, ErrorMessage = "El precio debe ser mayor que 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PRECIO { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [Display(Name = "Categoría")]
        public int ID_CATEGORIA { get; set; }

        // Propiedad de navegación a Categoria
        [ForeignKey("ID_CATEGORIA")]
        public virtual Categoria Categoria { get; set; }

        public string NOMBRE_CATEGORIA { get; set; }


        [Display(Name = "Imagen del juego")]
        [StringLength(300, ErrorMessage = "La URL de la imagen no debe superar los 300 caracteres.")]
        [Url(ErrorMessage = "Debe ingresar una URL válida para la imagen.")]
        public string IMAGEN_URL { get; set; }

        [Display(Name = "Video del juego")]
        [StringLength(300, ErrorMessage = "La URL del video no debe superar los 300 caracteres.")]
        [Url(ErrorMessage = "Debe ingresar una URL válida para el video.")]
        public string VIDEO_URL { get; set; }


        public bool ACTIVO { get; set; }

        // Relaciones
        public virtual ICollection<DetalleCompra> DetallesCompra { get; set; }
        public virtual ICollection<Recomendacion> Recomendaciones { get; set; }
    }

}
