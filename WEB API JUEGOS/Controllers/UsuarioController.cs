using ExperienciasProyecto.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using WEB_API_JUEGOS.Data.Contrato;
using WEB_API_JUEGOS.Models.Dto;

namespace WEB_API_JUEGOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuario _usuarioRepo;

        public UsuarioController(IUsuario usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }



        // POST api/usuario/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] UsuarioDto request)
        {
            var user = _usuarioRepo.login(request.CORREO, request.CONTRASENA);
            if (user == null)
                return Unauthorized(new { mensaje = "Credenciales inválidas" });

            return Ok(user);
        }

        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] UsuarioRegistroDto request)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                return BadRequest(new { mensaje = "Modelo inválido", errores });
            }

            var rolAsignado = string.IsNullOrEmpty(request.ROL) ? "Usuario" : request.ROL;

            bool registrado = _usuarioRepo.RegistrarUsuario(
                request.NOMBRE,
                request.CORREO,
                request.CONTRASENA,
                rolAsignado
            );

            if (!registrado)
                return BadRequest(new { mensaje = "⚠️ El correo ya está registrado." });

            return Ok(new { mensaje = "✅ Usuario registrado correctamente" });
        }




        // GET api/usuario/{id}/historial
        [HttpGet("{id}/historial")]
        public IActionResult Historial(int id)
        {
            var historial = _usuarioRepo.HistorialUsuario(id);
            return Ok(historial);
        }

        // GET api/usuario/{id}/totalgastado
        [HttpGet("{id}/totalgastado")]
        public IActionResult TotalGastado(int id)
        {
            var total = _usuarioRepo.TotalGastadoUsuario(id);
            return Ok(new { totalGastado = total });
        }


        // GET api/usuario/{id}/perfil
        [HttpGet("{id}/perfil")]
        public IActionResult Perfil(int id)
        {
            var perfil = _usuarioRepo.ObtenerPerfil(id);
            if (perfil == null)
                return NotFound(new { mensaje = "No se encontró el perfil" });

            return Ok(perfil);
        }


    }
}

