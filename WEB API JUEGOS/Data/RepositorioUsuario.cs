using System.Data;
using ExperienciasProyecto.Models;
using Microsoft.Data.SqlClient;
using WEB_API_JUEGOS.Data.Contrato;
using WEB_API_JUEGOS.Models.Dto;

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
        public bool RegistrarUsuario(string nombre, string correo, string contrasena, string rol)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();

                // 🔍 Verificar si ya existe el correo antes de insertar
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(1) FROM USUARIO WHERE CORREO = @CORREO", cn);
                checkCmd.Parameters.AddWithValue("@CORREO", correo);

                int existe = (int)checkCmd.ExecuteScalar();
                if (existe > 0)
                {
                    return false; // el correo ya está registrado
                }

                // Si no existe, recién ejecutamos el SP
                SqlCommand cmd = new SqlCommand("SP_REGISTRAR_USUARIO", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@NOMBRE", nombre);
                cmd.Parameters.AddWithValue("@CORREO", correo);
                cmd.Parameters.AddWithValue("@CONTRASENA", contrasena);
                cmd.Parameters.AddWithValue("@ROL", rol);

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

        public PerfilViewModel ObtenerPerfil(int idUsuario)
        {
            var perfil = new PerfilViewModel();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_PERFIL_USUARIO", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);

                SqlDataReader dr = cmd.ExecuteReader();

                // Usuario
                if (dr.Read())
                {
                    perfil.Usuario = new Usuario
                    {
                        ID_USUARIO = dr.GetInt32(dr.GetOrdinal("ID_USUARIO")),
                        NOMBRE = dr.GetString(dr.GetOrdinal("NOMBRE")),
                        CORREO = dr.GetString(dr.GetOrdinal("CORREO")),
                        ROL = dr.GetString(dr.GetOrdinal("ROL"))
                    };
                }

                // Historial de compras
                dr.NextResult();
                perfil.HistorialDeCompras = new List<HistorialCompra>();
                while (dr.Read())
                {
                    ((List<HistorialCompra>)perfil.HistorialDeCompras).Add(new HistorialCompra
                    {
                        ID_COMPRA = dr.GetInt32(dr.GetOrdinal("ID_COMPRA")),
                        FECHA = dr.GetDateTime(dr.GetOrdinal("FECHA")),
                        NOMBRE_JUEGO = dr.GetString(dr.GetOrdinal("NOMBRE_JUEGO")),
                        IMAGEN_URL = dr.IsDBNull(dr.GetOrdinal("IMAGEN_URL")) ? null : dr.GetString(dr.GetOrdinal("IMAGEN_URL")),
                        PRECIO_UNITARIO = dr.GetDecimal(dr.GetOrdinal("PRECIO_UNITARIO"))
                    });
                }

                // Total gastado
                dr.NextResult();
                if (dr.Read())
                {
                    int colIndex = dr.GetOrdinal("TOTAL_GASTADO");
                    perfil.TotalGastado = dr.IsDBNull(colIndex) ? 0 : dr.GetDecimal(colIndex);
                }

                // Recomendaciones
                dr.NextResult();
                perfil.Recomendaciones = new List<RecomendacionVista>();
                while (dr.Read())
                {
                    ((List<RecomendacionVista>)perfil.Recomendaciones).Add(new RecomendacionVista
                    {
                        ID_RECOMENDACION = dr.GetInt32(dr.GetOrdinal("ID_RECOMENDACION")),
                        NOMBRE_JUEGO = dr.GetString(dr.GetOrdinal("NOMBRE")),
                        MOTIVO = dr.GetString(dr.GetOrdinal("MOTIVO")),
                        IMAGEN_URL = dr.IsDBNull(dr.GetOrdinal("IMAGEN_URL")) ? null : dr.GetString(dr.GetOrdinal("IMAGEN_URL"))
                    });
                }

                dr.Close();
            }

            return perfil;
        }

    }
}
