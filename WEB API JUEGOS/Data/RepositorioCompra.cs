using System.Data;
using Microsoft.Data.SqlClient;
using WEB_API_JUEGOS.Data.Contrato;

namespace WEB_API_JUEGOS.Data
{
    public class RepositorioCompra : ICompra
    {

        private readonly string _connectionString;

       
        public RepositorioCompra(IConfiguration config)
        {
            _connectionString = config["ConnectionStrings:cadena"];
        }
        public int RegistrarCompra(int idUsuario, decimal total)
        {
            int idCompra = 0;
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_REGISTRAR_COMPRA", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);
                cmd.Parameters.AddWithValue("@TOTAL", total);

                SqlParameter outputParam = new SqlParameter("@ID_COMPRA", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                cmd.ExecuteNonQuery();
                idCompra = (int)outputParam.Value;
                cn.Close();
            }
            return idCompra;
        }

        public bool RegistrarDetalle(int idCompra, int idJuego, decimal precio)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_REGISTRAR_DETALLE_COMPRA", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_COMPRA", idCompra);
                cmd.Parameters.AddWithValue("@ID_JUEGO", idJuego);
                cmd.Parameters.AddWithValue("@PRECIO_UNITARIO", precio);

                int filas = cmd.ExecuteNonQuery();
                cn.Close();
                return filas > 0;
            }
        }
    }
}

