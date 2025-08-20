using System.Data;
using ExperienciasProyecto.Models;
using Microsoft.Data.SqlClient;
using WEB_API_JUEGOS.Data.Contrato;

namespace WEB_API_JUEGOS.Data
{
    public class RepositorioUsuario : IUsuario
    {

        private readonly string _connectionString;


        public RepositorioUsuario(IConfiguration config)
        {
            _connectionString = config["ConnectionStrings:cadena"];
        }



        public IEnumerable<HistorialCompra> HistorialUsuario(int idUsuario)
        {
            List<HistorialCompra> lista = new List<HistorialCompra>();
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_HISTORIAL_USUARIO", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new HistorialCompra()
                    {
                        ID_COMPRA = dr.GetInt32(0),
                        FECHA = dr.GetDateTime(1),
                        NOMBRE_JUEGO = dr.GetString(2),
                        IMAGEN_URL = dr.IsDBNull(3) ? null : dr.GetString(3),
                        PRECIO_UNITARIO = dr.GetDecimal(4)
                    });
                }
                dr.Close();
            }
            return lista;
        }

        public Usuario login(string correo, string contrasena)
        {
            Usuario user = null;
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_LOGIN", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CORREO", correo);
                cmd.Parameters.AddWithValue("@CONTRASENA", contrasena);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    user = new Usuario()
                    {
                        ID_USUARIO = dr.GetInt32(0),
                        NOMBRE = dr.GetString(1),
                        CORREO = dr.GetString(2),
                        CONTRASENA = dr.GetString(3),
                        ROL = dr.IsDBNull(4) ? "Usuario" : dr.GetString(4)
                    };
                }
                dr.Close();
            }
            return user;
        }

        public bool RegistrarUsuario(string nombre, string correo, string contrasena)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_REGISTRAR_USUARIO", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NOMBRE", nombre);
                cmd.Parameters.AddWithValue("@CORREO", correo);
                cmd.Parameters.AddWithValue("@CONTRASENA", contrasena);

                int filas = cmd.ExecuteNonQuery();
                return filas > 0;
            }
        }

        public decimal TotalGastadoUsuario(int idUsuario)
        {
            decimal total = 0;
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_TOTAL_GASTADO_USUARIO", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    total = dr.IsDBNull(0) ? 0 : dr.GetDecimal(0);
                }
                dr.Close();
            }
            return total;
        }
    }
}
