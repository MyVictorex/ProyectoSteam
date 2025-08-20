using ExperienciasProyecto.Models;

namespace WEB_API_JUEGOS.Data.Contrato
{
    public interface IJuego
    {
        IEnumerable<Juego> BuscarJuegos(string busqueda);
        IEnumerable<RecomendacionVista> VerRecomendaciones(int idUsuario);
        IEnumerable<Juego> ObtenerJuegos();
        void InsertarJuegoBD(Juego juego);
        void EditarJuegoBD(Juego juego);
        void DesactivarJuegoBD(int id);
        void ActivarjuegoBD(int id);
        List<Juego> ObtenerTodosLosJuegos();
        void InsertarRecomendacion(int idUsuario, int idJuego, string motivo);
    }
}
