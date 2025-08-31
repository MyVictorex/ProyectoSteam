using System.ComponentModel.DataAnnotations;

namespace WEB_API_JUEGOS.Models.Dto
{
    public class UsuarioRegistroDto
    {
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

        public string ROL { get; set; }
    }
}