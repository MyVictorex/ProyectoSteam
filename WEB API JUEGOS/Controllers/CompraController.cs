using Microsoft.AspNetCore.Mvc;
using WEB_API_JUEGOS.Data;
using WEB_API_JUEGOS.Data.Contrato;
using WEB_API_JUEGOS.Models.Dto;

namespace WEB_API_JUEGOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        private readonly ICompra _compraRepo;

        public CompraController(ICompra compraRepo)
        {
            _compraRepo = compraRepo;
        }

        [HttpPost("registrar")]
        public IActionResult RegistrarCompra([FromBody] CompraDto request)
        {
            if (request == null || request.ID_USUARIO <= 0 || request.DetallesCompra == null || !request.DetallesCompra.Any() || request.TOTAL <= 0)
                return BadRequest("Datos inválidos");

            try
            {
                // Registrar la compra y obtener el ID real generado
                int idCompra = _compraRepo.RegistrarCompra(request.ID_USUARIO, request.TOTAL);

                foreach (var detalle in request.DetallesCompra)
                {
                    _compraRepo.RegistrarDetalle(idCompra, detalle.ID_JUEGO, detalle.PRECIO_UNITARIO);
                }

                return Ok(new { idCompra });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
