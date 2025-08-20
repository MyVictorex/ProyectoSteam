using ExperienciasProyecto.Models;

namespace WEB_API_JUEGOS.Data.Contrato
{
    public interface IUsuario
    {

        Usuario login(string correo, string contrasena);
        bool RegistrarUsuario(string nombre, string correo, string contrasena);
        IEnumerable<HistorialCompra> HistorialUsuario(int idUsuario);
        decimal TotalGastadoUsuario(int idUsuario);
    }
}
