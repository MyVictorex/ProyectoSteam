using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using WEB_API_JUEGOS.Data.Contrato;

namespace WEB_API_JUEGOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {

        private readonly ICategoria _categoriaRepo;

        public CategoriaController(ICategoria categoriaRepo)
        {
            _categoriaRepo = categoriaRepo;
        }

        [HttpGet]
        public IActionResult GetCategorias()
        {
            var categorias = _categoriaRepo.ObtenerTodas();
            return Ok(categorias);
        }
    }
}
