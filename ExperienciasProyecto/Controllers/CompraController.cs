using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExperienciasProyecto.Models;
using Newtonsoft.Json;
using WEB_API_JUEGOS.Models.Dto;

namespace ExperienciasProyecto.Controllers
{
    public class CompraController : Controller
    {
        private readonly string apiUrl = ConfigurationManager.AppSettings["URL_API"];

        private async Task<int> registrarCompraAsync(CompraDto request)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var json = JsonConvert.SerializeObject(request, Formatting.Indented);
                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                var mensaje = await clienteHttp.PostAsync("compra/registrar", contenido);
                if (!mensaje.IsSuccessStatusCode)
                {
                    var error = await mensaje.Content.ReadAsStringAsync();
                    Console.WriteLine("Error API: " + error);
                    return 0;
                }

                var data = await mensaje.Content.ReadAsStringAsync();
                dynamic response = JsonConvert.DeserializeObject(data);
                return (int)response.idCompra;
            }
        }
        private async Task<bool> insertarRecomendacionAsync(int idUsuario, int idJuego, string motivo)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var resp = await clienteHttp.PostAsync(
                    $"juegos/recomendacion?idUsuario={idUsuario}&idJuego={idJuego}&motivo={motivo}", null);
                return resp.IsSuccessStatusCode;
            }
        }

        private async Task<List<HistorialCompra>> obtenerHistorialAsync(int idUsuario)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var mensaje = await clienteHttp.GetAsync($"usuario/{idUsuario}/historial");
                if (!mensaje.IsSuccessStatusCode) return new List<HistorialCompra>();

                var data = await mensaje.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<HistorialCompra>>(data);
            }
        }

        // ==== Acciones visibles ====

        public ActionResult Carrito()
        {
            if (Session["ID_USUARIO"] == null)
                return RedirectToAction("Login", "Usuario");

            var carrito = Session["carrito"] as List<CarritoItem> ?? new List<CarritoItem>();
            return View(carrito);
        }

        [HttpPost]
        public async Task<ActionResult> Comprar()
        {
            if (Session["ID_USUARIO"] == null)
                return RedirectToAction("Login", "Usuario");

            int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
            var carrito = Session["carrito"] as List<CarritoItem>;

            if (carrito == null || !carrito.Any())
                return RedirectToAction("Carrito");

            // Crear DTO
            var compraDto = new CompraDto
            {
                ID_USUARIO = idUsuario,
                TOTAL = carrito.Sum(i => i.PRECIO),
                DetallesCompra = carrito.Select(i => new DetalleCompraDto
                {
                    ID_JUEGO = i.ID_JUEGO,
                    PRECIO_UNITARIO = i.PRECIO
                }).ToList()
            };

            int idCompra = await registrarCompraAsync(compraDto);

            if (idCompra == 0)
            {
                TempData["MensajeCompra"] = "No se pudo registrar la compra. Revisa la API.";
                return RedirectToAction("Carrito");
            }

            // Registrar recomendaciones
            foreach (var item in carrito)
                await insertarRecomendacionAsync(idUsuario, item.ID_JUEGO, "Basado en tu reciente compra");

            // Limpiar carrito de sesión
            Session.Remove("carrito");

            // Limpiar cookie si existe
            if (Request.Cookies[$"carrito_{idUsuario}"] != null)
            {
                var expiredCookie = new HttpCookie($"carrito_{idUsuario}");
                expiredCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(expiredCookie);
            }

            TempData["MensajeCompra"] = $"Compra registrada exitosamente. ID_COMPRA={idCompra}";
            return RedirectToAction("Historial");
        }




        public async Task<ActionResult> Historial()
        {
            if (Session["ID_USUARIO"] == null)
                return RedirectToAction("Login", "Usuario");

            int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
            var historial = await obtenerHistorialAsync(idUsuario);

            return View(historial);
        }


        public async Task<ActionResult> AgregarAlCarrito(int id)
        {
            if (Session["ID_USUARIO"] == null)
            {
                TempData["MensajeCompra"] = "Debes iniciar sesión para agregar juegos al carrito.";
                return RedirectToAction("Login", "Usuario");
            }

            // Obtener detalle del juego desde la API
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var resp = await clienteHttp.GetAsync($"juegos/{id}");
                if (!resp.IsSuccessStatusCode)
                {
                    TempData["MensajeCompra"] = "No se pudo encontrar el juego.";
                    return RedirectToAction("Index", "Juego");
                }

                var data = await resp.Content.ReadAsStringAsync();
                var juego = JsonConvert.DeserializeObject<Juego>(data);

                if (juego != null)
                {
                    var item = new CarritoItem
                    {
                        ID_JUEGO = juego.ID_JUEGO,
                        NOMBRE = juego.NOMBRE,
                        PRECIO = juego.PRECIO,
                        IMAGEN_URL = juego.IMAGEN_URL
                    };

                    // Recuperar o crear carrito
                    List<CarritoItem> carrito = Session["carrito"] as List<CarritoItem> ?? new List<CarritoItem>();
                    carrito.Add(item);
                    Session["carrito"] = carrito;

                    // Guardar en cookie (persistencia)
                    int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
                    string carritoJson = JsonConvert.SerializeObject(carrito);

                    var cookie = new HttpCookie($"carrito_{idUsuario}", carritoJson);
                    cookie.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(cookie);
                }
            }

            TempData["MensajeCompra"] = "Juego agregado al carrito correctamente.";
            return RedirectToAction("Carrito");
        }
        public ActionResult QuitarDelCarrito(int id)
        {
            if (Session["ID_USUARIO"] == null)
            {
                TempData["MensajeCompra"] = "Debes iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            List<CarritoItem> carrito = Session["carrito"] as List<CarritoItem>;

            if (carrito != null)
            {
                var item = carrito.FirstOrDefault(c => c.ID_JUEGO == id);
                if (item != null)
                {
                    carrito.Remove(item);
                    Session["carrito"] = carrito;

                    // Actualizar cookie también
                    int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
                    string carritoJson = JsonConvert.SerializeObject(carrito);

                    var cookie = new HttpCookie($"carrito_{idUsuario}", carritoJson);
                    cookie.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(cookie);

                    TempData["MensajeCompra"] = $"Se quitó '{item.NOMBRE}' del carrito.";
                }
            }

            return RedirectToAction("Carrito");
        }

    }
}
