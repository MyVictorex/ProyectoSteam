using ExperienciasProyecto.Models;
using WEB_API_JUEGOS.Models.Dto;

namespace WEB_API_JUEGOS.Data.Contrato
{
    public interface IJuego
    {
        IEnumerable<Juego> BuscarJuegos(string busqueda);
        IEnumerable<RecomendacionVista> VerRecomendaciones(int idUsuario);
        IEnumerable<Juego> ObtenerJuegos();
        void InsertarJuegoBD(JuegoRegistroDto juego);
        void EditarJuegoBD(JuegoRegistroDto juego);
        void DesactivarJuegoBD(int id);
        void ActivarjuegoBD(int id);
        List<Juego> ObtenerTodosLosJuegos();
        void InsertarRecomendacion(int idUsuario, int idJuego, string motivo);
    }
}
