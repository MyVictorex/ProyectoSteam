using ExperienciasProyecto.Models;

namespace WEB_API_JUEGOS.Data.Contrato
{
    public interface ICategoria
    {
        List<Categoria> ObtenerTodas();

     
        Categoria ObtenerPorId(int id);
    }
}

