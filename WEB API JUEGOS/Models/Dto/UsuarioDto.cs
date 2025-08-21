using System.ComponentModel.DataAnnotations;

namespace WEB_API_JUEGOS.Models.Dto
{
    public class UsuarioDto
    {
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string CORREO { get; set; }

        [Required]
        [StringLength(100)]
        public string CONTRASENA { get; set; }
    }
}
