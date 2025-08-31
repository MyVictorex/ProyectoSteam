using System.Data;
using ExperienciasProyecto.Models;
using Microsoft.Data.SqlClient;
using WEB_API_JUEGOS.Data.Contrato;
using WEB_API_JUEGOS.Models.Dto;

namespace WEB_API_JUEGOS.Data
{
    public class RepositorioJuego : IJuego
    {
        private readonly string _connectionString;


        public RepositorioJuego(IConfiguration config)
        {
            _connectionString = config["ConnectionStrings:cadena"];
        }

        public IEnumerable<Juego> ObtenerJuegos()
        {
            List<Juego> juegos = new List<Juego>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_LISTAR_JUEGOS", cn); // solo activos
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    juegos.Add(new Juego
                    {
                        ID_JUEGO = dr.GetInt32(0),
                        NOMBRE = dr.GetString(1),
                        DESCRIPCION = dr.GetString(2),
                        PRECIO = dr.GetDecimal(3),
                   
                        ACTIVO = dr.GetBoolean(4), // importante
                        IMAGEN_URL = dr.IsDBNull(5) ? null : dr.GetString(5),
                        VIDEO_URL = dr.IsDBNull(6) ? null : dr.GetString(6),
                        NOMBRE_CATEGORIA = dr.GetString(7),
                    });
                }
                dr.Close();
                cn.Close();
            }

            return juegos;
        }

        public List<Juego> ObtenerTodosLosJuegos()
        {
            List<Juego> juegos = new List<Juego>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ObtenerJuegosParaVista", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    juegos.Add(new Juego
                    {
                        ID_JUEGO = dr.GetInt32(0),
                        NOMBRE = dr.GetString(1),
                        DESCRIPCION = dr.GetString(2),
                        PRECIO = dr.GetDecimal(3),
                        ACTIVO = dr.GetBoolean(4),
                        IMAGEN_URL = dr.IsDBNull(5) ? null : dr.GetString(5),
                        ID_CATEGORIA = dr.GetInt32(6),
                        NOMBRE_CATEGORIA = dr.IsDBNull(7) ? null : dr.GetString(7) // <--- Esto es clave
                    });
                }
                dr.Close();
                cn.Close();
            }

            return juegos;
        }




        public IEnumerable<Juego> BuscarJuegos(string busqueda)
        {
            List<Juego> juegos = new List<Juego>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_BUSCAR_JUEGOS", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@BUSQUEDA", busqueda);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    juegos.Add(new Juego
                    {
                        ID_JUEGO = dr.GetInt32(0),
                        NOMBRE = dr.GetString(1),
                        DESCRIPCION = dr.GetString(2),
                        PRECIO = dr.GetDecimal(3),
                        ID_CATEGORIA = dr.GetInt32(4),
                        IMAGEN_URL = dr.IsDBNull(5) ? null : dr.GetString(5)
                    });
                }

                dr.Close();
                cn.Close();
            }

            return juegos;
        }

        public void InsertarJuegoBD(JuegoRegistroDto juego)
        {
            if (juego == null)
                throw new ArgumentNullException(nameof(juego), "El objeto juego no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(juego.NOMBRE) ||
                string.IsNullOrWhiteSpace(juego.DESCRIPCION) ||
                juego.ID_CATEGORIA <= 0 ||   // <-- Validar ID de categoría
                juego.PRECIO <= 0)
            {
                throw new ArgumentException("Todos los campos obligatorios del juego deben estar correctamente llenos.");
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(_connectionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SP_INSERTAR_JUEGO", cn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@NOMBRE", juego.NOMBRE);
                    cmd.Parameters.AddWithValue("@DESCRIPCION", juego.DESCRIPCION);
                    cmd.Parameters.AddWithValue("@PRECIO", juego.PRECIO);
                    cmd.Parameters.AddWithValue("@ID_CATEGORIA", juego.ID_CATEGORIA); // <-- Cambiado
                    cmd.Parameters.AddWithValue("@IMAGEN_URL", juego.IMAGEN_URL ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VIDEO_URL", juego.VIDEO_URL ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al insertar el juego en la base de datos: " + ex.Message);
            }
        }

        public void EditarJuegoBD(JuegoRegistroDto juego)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_EDITAR_JUEGO", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@ID_JUEGO", juego.ID_JUEGO);
                cmd.Parameters.AddWithValue("@NOMBRE", juego.NOMBRE);
                cmd.Parameters.AddWithValue("@DESCRIPCION", juego.DESCRIPCION);
                cmd.Parameters.AddWithValue("@PRECIO", juego.PRECIO);
                cmd.Parameters.AddWithValue("@ID_CATEGORIA", juego.ID_CATEGORIA);
                cmd.Parameters.AddWithValue("@IMAGEN_URL", juego.IMAGEN_URL ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VIDEO_URL", juego.VIDEO_URL ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }



        public void DesactivarJuegoBD(int id)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_DESACTIVAR_JUEGO", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_JUEGO", id);
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        public void ActivarjuegoBD(int id)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_ACTIVAR_JUEGO", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_JUEGO", id);
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<RecomendacionVista> VerRecomendaciones(int idUsuario)
        {
            List<RecomendacionVista> lista = new List<RecomendacionVista>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_VER_RECOMENDACIONES", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new RecomendacionVista()
                    {
                        ID_RECOMENDACION = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
                        NOMBRE_JUEGO = dr.IsDBNull(1) ? "Sin nombre" : dr.GetString(1),
                        MOTIVO = dr.IsDBNull(2) ? "Sin motivo" : dr.GetString(2),
                        IMAGEN_URL = dr.IsDBNull(3) ? null : dr.GetString(3)
                    });
                }

                dr.Close();
                cn.Close();
            }

            return lista;
        }

        public void InsertarRecomendacion(int idUsuario, int idJuego, string motivo)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO RECOMENDACION (ID_USUARIO, ID_JUEGO, MOTIVO) VALUES (@ID_USUARIO, @ID_JUEGO, @MOTIVO)", cn);
                cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);
                cmd.Parameters.AddWithValue("@ID_JUEGO", idJuego);
                cmd.Parameters.AddWithValue("@MOTIVO", motivo);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
