using ExperienciasProyecto.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WEB_API_JUEGOS.Data.Contrato;
using WEB_API_JUEGOS.Models.Dto;

namespace WEB_API_JUEGOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JuegosController : ControllerBase
    {

        private readonly IJuego _juegoRepo;

        public JuegosController(IJuego juegoRepo)
        {
            _juegoRepo = juegoRepo;
        }


        [HttpGet]
        public IActionResult ObtenerJuegosActivos()
        {
            var juegos = _juegoRepo.ObtenerJuegos();
            return Ok(juegos);
        }

        // GET api/juegos/todos
        [HttpGet("todos")]
        public IActionResult ObtenerTodos()
        {
            var juegos = _juegoRepo.ObtenerTodosLosJuegos();
            return Ok(juegos);
        }

        // GET api/juegos/buscar?busqueda=accion
        [HttpGet("buscar")]
        public IActionResult Buscar([FromQuery] string busqueda)
        {
            var juegos = _juegoRepo.BuscarJuegos(busqueda);
            return Ok(juegos);
        }

       
        [HttpPost("recomendacion")]
        public IActionResult InsertarRecomendacion(int idUsuario, int idJuego, string motivo)
        {
            _juegoRepo.InsertarRecomendacion(idUsuario, idJuego, motivo);
            return Ok(new { mensaje = "Recomendación insertada correctamente" });
        }

        // POST api/juegos
        [HttpPost]
        public IActionResult Insertar(JuegoRegistroDto juego)
        {
            _juegoRepo.InsertarJuegoBD(juego);
            return Ok(new { mensaje = "Juego insertado correctamente" });
        }

        // PUT api/juegos
        [HttpPut]
        public IActionResult Editar(JuegoRegistroDto juego)
        {
            _juegoRepo.EditarJuegoBD(juego);
            return Ok(new { mensaje = "Juego editado correctamente" });
        }

        // PUT api/juegos/desactivar/5
        [HttpPut("desactivar/{id}")]
        public IActionResult Desactivar(int id)
        {
            _juegoRepo.DesactivarJuegoBD(id);
            return Ok(new { mensaje = "Juego desactivado correctamente" });
        }

        // PUT api/juegos/activar/5
        [HttpPut("activar/{id}")]
        public IActionResult Activar(int id)
        {
            _juegoRepo.ActivarjuegoBD(id);
            return Ok(new { mensaje = "Juego activado correctamente" });
        }

        // GET api/juegos/recomendaciones/3
        [HttpGet("recomendaciones/{idUsuario}")]
        public IActionResult VerRecomendaciones(int idUsuario)
        {
            var lista = _juegoRepo.VerRecomendaciones(idUsuario);
            return Ok(lista);
        }

        // GET api/juegos/5
        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var juego = _juegoRepo.ObtenerJuegos().FirstOrDefault(j => j.ID_JUEGO == id);
            if (juego == null)
                return NotFound();
            return Ok(juego);
        }

    }
}
