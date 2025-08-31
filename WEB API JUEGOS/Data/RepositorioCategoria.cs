using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ExperienciasProyecto.Models;
using WEB_API_JUEGOS.Data.Contrato;


namespace WEB_API_JUEGOS.Data
{
    public class RepositorioCategoria : ICategoria
    {
        private readonly string _connectionString;


        public RepositorioCategoria(IConfiguration config)
        {
            _connectionString = config["ConnectionStrings:cadena"];
        }

        List<Categoria> ICategoria.ObtenerTodas()
        {
            var categorias = new List<Categoria>();

            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var cmd = new SqlCommand("SELECT ID_CATEGORIA, NOMBRE FROM Categoria", cn);
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    categorias.Add(new Categoria
                    {
                        ID_CATEGORIA = dr.GetInt32(0),
                        NOMBRE = dr.GetString(1)
                    });
                }
            }

            return categorias;
        }

        public Categoria ObtenerPorId(int id)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var cmd = new SqlCommand("SELECT ID_CATEGORIA, NOMBRE FROM Categoria WHERE ID_CATEGORIA=@id", cn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new Categoria
                    {
                        ID_CATEGORIA = dr.GetInt32(0),
                        NOMBRE = dr.GetString(1)
                    };
                }

                return null;
            }
        }

      
    }
}
