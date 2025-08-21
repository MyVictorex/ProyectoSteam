using ExperienciasProyecto.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WEB_API_JUEGOS.Data;

namespace WEB_API_JUEGOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        private readonly RepositorioCompra _compraRepo;

        public CompraController(RepositorioCompra compraRepo)
        {
            _compraRepo = compraRepo;
        }

        // POST api/compra/registrar
        [HttpPost("registrar")]
        public IActionResult RegistrarCompra([FromBody] Compra request)
        {
            if (request == null || request.DetallesCompra == null || !request.DetallesCompra.Any())
                return BadRequest(new { mensaje = "La compra debe tener detalles." });

            try
            {
                // 1. Registrar compra principal
                int idCompra = _compraRepo.RegistrarCompra(request.ID_USUARIO, request.TOTAL);

                // 2. Registrar los detalles
                foreach (var detalle in request.DetallesCompra)
                {
                    _compraRepo.RegistrarDetalle(idCompra, detalle.ID_JUEGO, detalle.PRECIO_UNITARIO);
                }

                return Ok(new { mensaje = "Compra registrada con éxito", idCompra });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al registrar la compra", error = ex.Message });
            }
        }
    }
}
