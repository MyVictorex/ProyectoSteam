using ExperienciasProyecto.Models;
using WEB_API_JUEGOS.Models.Dto;

namespace WEB_API_JUEGOS.Data.Contrato
{
    public interface IUsuario
    {

        Usuario login(string correo, string contrasena);
        bool RegistrarUsuario(string nombre, string correo, string contrasena,string rol);
        IEnumerable<HistorialCompra> HistorialUsuario(int idUsuario);
        decimal TotalGastadoUsuario(int idUsuario);

        PerfilViewModel ObtenerPerfil(int idUsuario);
    }
}
