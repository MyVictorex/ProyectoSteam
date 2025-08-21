using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExperienciasProyecto.Models;
using Newtonsoft.Json;

namespace ExperienciasProyecto.Controllers
{
    public class JuegoController : Controller
    {

        public Usuario login(string correo, string contrasena)
        {
            Usuario user = null;

            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
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
                cn.Close();
            }

            return user;
        }
        public bool RegistrarUsuario(string nombre, string correo, string contrasena)
        {
            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_REGISTRAR_USUARIO", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@NOMBRE", nombre);
                cmd.Parameters.AddWithValue("@CORREO", correo);
                cmd.Parameters.AddWithValue("@CONTRASENA", contrasena);

                int filasAfectadas = cmd.ExecuteNonQuery();
                cn.Close();

                return filasAfectadas > 0; 
            }
        }

        public IEnumerable<HistorialCompra> HistorialUsuario(int idUsuario)
        {
            List<HistorialCompra> lista = new List<HistorialCompra>();

            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_HISTORIAL_USUARIO", cn);
                cmd.CommandType = CommandType.StoredProcedure; // ✅ Esto faltaba
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
                cn.Close();
            }

            return lista;
        }


        public int RegistrarCompra(int idUsuario, decimal total)
        {
            int idCompra = 0;

            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
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
            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
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

        public decimal TotalGastadoUsuario(int idUsuario)
        {
            decimal total = 0;

            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
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
                cn.Close();
            }

            return total;
        }

        public IEnumerable<Juego> BuscarJuegos(string busqueda)
        {
            List<Juego> juegos = new List<Juego>();

            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
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
                        CATEGORIA = dr.GetString(4),
                        IMAGEN_URL = dr.IsDBNull(5) ? null : dr.GetString(5) 
                    });
                }

                dr.Close();
                cn.Close();
            }

            return juegos;
        }


        public IEnumerable<RecomendacionVista> VerRecomendaciones(int idUsuario)
        {
            List<RecomendacionVista> lista = new List<RecomendacionVista>();

            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
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


        public IEnumerable<Juego> ObtenerJuegos()
        {
            List<Juego> juegos = new List<Juego>();

            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
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
                        CATEGORIA = dr.GetString(4),
                        ACTIVO = dr.GetBoolean(5), // importante
                        IMAGEN_URL = dr.IsDBNull(6) ? null : dr.GetString(6),
                        VIDEO_URL = dr.IsDBNull(7) ? null : dr.GetString(7)

                    });
                }
                dr.Close();
                cn.Close();
            }

            return juegos;
        }


        public void InsertarJuegoBD(Juego juego)
        {
            if (juego == null)
                throw new ArgumentNullException(nameof(juego), "El objeto juego no puede ser nulo.");

            // Validación básica opcional
            if (string.IsNullOrWhiteSpace(juego.NOMBRE) ||
                string.IsNullOrWhiteSpace(juego.DESCRIPCION) ||
                string.IsNullOrWhiteSpace(juego.CATEGORIA) ||
                juego.PRECIO <= 0)
            {
                throw new ArgumentException("Todos los campos obligatorios del juego deben estar correctamente llenos.");
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SP_INSERTAR_JUEGO", cn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@NOMBRE", juego.NOMBRE);
                    cmd.Parameters.AddWithValue("@DESCRIPCION", juego.DESCRIPCION);
                    cmd.Parameters.AddWithValue("@PRECIO", juego.PRECIO);
                    cmd.Parameters.AddWithValue("@CATEGORIA", juego.CATEGORIA);
                    cmd.Parameters.AddWithValue("@IMAGEN_URL", juego.IMAGEN_URL ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VIDEO_URL", juego.VIDEO_URL ?? (object)DBNull.Value);



                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                // Log de error o mensaje informativo (puedes adaptarlo)
                throw new Exception("Error al insertar el juego en la base de datos: " + ex.Message);
            }
        }


        public void EditarJuegoBD(Juego juego)
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_EDITAR_JUEGO", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_JUEGO", juego.ID_JUEGO);
                cmd.Parameters.AddWithValue("@NOMBRE", juego.NOMBRE);
                cmd.Parameters.AddWithValue("@DESCRIPCION", juego.DESCRIPCION);
                cmd.Parameters.AddWithValue("@PRECIO", juego.PRECIO);
                cmd.Parameters.AddWithValue("@CATEGORIA", juego.CATEGORIA);

                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        public void DesactivarJuegoBD(int id)
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
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
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SP_ACTIVAR_JUEGO", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_JUEGO", id);
                cmd.ExecuteNonQuery();
            }
        }
        public List<Juego> ObtenerTodosLosJuegos()
        {
            List<Juego> juegos = new List<Juego>();

            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM JUEGO", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    juegos.Add(new Juego
                    {
                        ID_JUEGO = dr.GetInt32(0),
                        NOMBRE = dr.GetString(1),
                        DESCRIPCION = dr.GetString(2),
                        PRECIO = dr.GetDecimal(3),
                        CATEGORIA = dr.GetString(4),
                        ACTIVO = dr.GetBoolean(5), // Asegúrate de tener esta propiedad
                        IMAGEN_URL = dr.IsDBNull(6) ? null : dr.GetString(6)
                    });
                }
                dr.Close();
                cn.Close();
            }

            return juegos;
        }






        public void InsertarRecomendacion(int idUsuario, int idJuego, string motivo)
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO RECOMENDACION (ID_USUARIO, ID_JUEGO, MOTIVO) VALUES (@ID_USUARIO, @ID_JUEGO, @MOTIVO)", cn);
                cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);
                cmd.Parameters.AddWithValue("@ID_JUEGO", idJuego);
                cmd.Parameters.AddWithValue("@MOTIVO", motivo);
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }


        //--------------------------------------------------------------
        public ActionResult Index()
        {
            // Recuperar carrito si no está en sesión
            if (Session["carrito"] == null && Session["ID_USUARIO"] != null)
            {
                int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
                var cookie = Request.Cookies[$"carrito_{idUsuario}"];
                if (cookie != null)
                {
                    var carrito = JsonConvert.DeserializeObject<List<CarritoItem>>(cookie.Value);
                    Session["carrito"] = carrito;
                }
            }

            // Verificar rol del usuario para decidir qué juegos mostrar
            List<Juego> juegos;

            if (Session["ROL"] != null && Session["ROL"].ToString() == "Admin")
            {
                // Mostrar todos los juegos (activos e inactivos)
                juegos = ObtenerTodosLosJuegos().ToList(); // 👈 Convertimos explícitamente a List
            }
            else
            {
                // Solo mostrar juegos activos
                juegos = ObtenerJuegos().ToList(); // 👈 Convertimos explícitamente a List
            }


            return View(juegos);
        }





        public ActionResult Historial()
        {
            if (Session["ID_USUARIO"] == null)
                return RedirectToAction("Login");

            int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
            var historial = HistorialUsuario(idUsuario);
            return View(historial);
        }




        [HttpPost]
        public ActionResult Login(string correo, string contrasena)
        {
            Usuario u = login(correo, contrasena);
            if (u != null)
            {
                Session["ID_USUARIO"] = u.ID_USUARIO;
                Session["NOMBRE_USUARIO"] = u.NOMBRE;
                Session["ROL"] = u.ROL;

                var cookie = Request.Cookies[$"carrito_{u.ID_USUARIO}"];
                if (cookie != null)
                {
                    var carrito = JsonConvert.DeserializeObject<List<CarritoItem>>(cookie.Value);
                    Session["carrito"] = carrito;
                }


                return RedirectToAction("Index");
            }

            ViewBag.Mensaje = "Correo o contraseña inválidos.";
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Buscar(string busqueda)
        {
            var juegos = BuscarJuegos(busqueda);
            return View(juegos);
        }
        public ActionResult TotalGastado()
        {
            if (Session["ID_USUARIO"] == null)
                return RedirectToAction("Login");

            int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
            ViewBag.TotalGastado = TotalGastadoUsuario(idUsuario);
            return View();
        }

        public ActionResult Recomendaciones()
        {
            if (Session["ID_USUARIO"] == null)
                return RedirectToAction("Login");

            int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
            var recomendaciones = VerRecomendaciones(idUsuario);
            return View(recomendaciones);
        }



        public ActionResult AgregarAlCarrito(int id)
        {
            // Validación: ¿El usuario está logueado?
            if (Session["ID_USUARIO"] == null)
            {
                TempData["MensajeCompra"] = "Debes iniciar sesión para agregar juegos al carrito.";
                return RedirectToAction("Login", "Juego");
            }

            // Buscar el juego por ID
            var juego = ObtenerJuegos().FirstOrDefault(j => j.ID_JUEGO == id);
            if (juego != null)
            {
                var item = new CarritoItem
                {
                    ID_JUEGO = juego.ID_JUEGO,
                    NOMBRE = juego.NOMBRE,
                    PRECIO = juego.PRECIO,
                    IMAGEN_URL = juego.IMAGEN_URL
                };

                // Obtener o crear el carrito
                List<CarritoItem> carrito = Session["carrito"] as List<CarritoItem> ?? new List<CarritoItem>();
                carrito.Add(item);
                Session["carrito"] = carrito;

                // Guardar en cookie si está logueado
                int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
                string carritoJson = JsonConvert.SerializeObject(carrito);

                var cookie = new HttpCookie($"carrito_{idUsuario}", carritoJson);
                cookie.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(cookie);
            }

            TempData["MensajeCompra"] = "Juego agregado al carrito correctamente.";
            return RedirectToAction("Index");
        }



        public ActionResult Carrito()
        {
            if (Session["ID_USUARIO"] == null)
                return RedirectToAction("Login");

            var carrito = Session["carrito"] as List<CarritoItem> ?? new List<CarritoItem>();
            return View(carrito);
        }

        [HttpPost]
        public ActionResult Comprar()
        {
            if (Session["ID_USUARIO"] == null)
                return RedirectToAction("Login");

            int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
            var carrito = Session["carrito"] as List<CarritoItem>;

            if (carrito == null || !carrito.Any())
                return RedirectToAction("Carrito");

            // Registrar la compra
            decimal total = carrito.Sum(i => i.PRECIO);
            int idCompra = RegistrarCompra(idUsuario, total);

            // Registrar detalles
            foreach (var item in carrito)
            {
                RegistrarDetalle(idCompra, item.ID_JUEGO, item.PRECIO);
            }
            // Registrar detalles
            foreach (var item in carrito)
            {
                RegistrarDetalle(idCompra, item.ID_JUEGO, item.PRECIO);

                // Insertar recomendación (después del detalle)
                InsertarRecomendacion(idUsuario, item.ID_JUEGO, "Basado en tu reciente compra");
            }


            // Limpiar carrito de sesión
            Session.Remove("carrito");

            // Eliminar cookie del carrito del usuario
            if (Request.Cookies[$"carrito_{idUsuario}"] != null)
            {
                var expiredCookie = new HttpCookie($"carrito_{idUsuario}");
                expiredCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(expiredCookie);
            }

            TempData["MensajeCompra"] = "Compra registrada exitosamente.";
            return RedirectToAction("Index");
        }





        [HttpGet]
        public ActionResult RegistrarUsu()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegistrarUsu(string nombre, string correo, string contrasena)
        {
            bool registrado = RegistrarUsuario(nombre, correo, contrasena);

            if (registrado)
            {
                ViewBag.Mensaje = "Usuario registrado exitosamente.";
                return RedirectToAction("Login"); // O redirige donde gustes
            }

            ViewBag.Mensaje = "Error al registrar usuario.";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult InsertarJuego()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InsertarJuego(Juego juego)
        {
            InsertarJuegoBD(juego);
            TempData["mensaje"] = "Juego insertado correctamente.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult EditarJuego(int id)
        {
            var juego = ObtenerJuegos().FirstOrDefault(j => j.ID_JUEGO == id);
            return View(juego);
        }

        [HttpPost]
        public ActionResult EditarJuego(Juego juego)
        {
            EditarJuegoBD(juego);
            TempData["mensaje"] = "Juego editado correctamente.";
            return RedirectToAction("Index");
        }

        // GET: Juego/DesactivarJuego/5
        public ActionResult DesactivarJuego(int id)
        {
            DesactivarJuegoBD(id); // Tu método que llama a SP_DESACTIVAR_JUEGO
            return RedirectToAction("Index");
        }

        // GET: Juego/ActivarJuego/5
        public ActionResult ActivarJuego(int id)
        {
            ActivarjuegoBD(id); // Tu método que llama a SP_ACTIVAR_JUEGO
            return RedirectToAction("Index");
        }

        public ActionResult Detalles(int id)
        {
            var juego = ObtenerJuegos().FirstOrDefault(j => j.ID_JUEGO == id);

            if (juego == null)
                return HttpNotFound();

            // Si está logueado, insertamos recomendación
            if (Session["ID_USUARIO"] != null)
            {
                int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
                InsertarRecomendacion(idUsuario, juego.ID_JUEGO, "Visto recientemente");
            }

            return View(juego);
        }
    }
}

