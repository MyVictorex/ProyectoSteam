using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExperienciasProyecto.Models;
using Newtonsoft.Json;
using WEB_API_JUEGOS.Models.Dto;

namespace ExperienciasProyecto.Controllers
{
    public class JuegoController : Controller
    {
        private readonly string apiUrl = ConfigurationManager.AppSettings["URL_API"];

        // ==== MÉTODOS PRIVADOS PARA CONSUMIR LA API ====

        private async Task<List<Juego>> obtenerJuegosActivosAsync()
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var resp = await clienteHttp.GetAsync("juegos");
                if (!resp.IsSuccessStatusCode) return new List<Juego>();
                var data = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Juego>>(data);
            }
        }

        private async Task<List<Juego>> obtenerTodosLosJuegosAsync()
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var resp = await clienteHttp.GetAsync("juegos/todos");
                if (!resp.IsSuccessStatusCode) return new List<Juego>();
                var data = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Juego>>(data);
            }
        }

        private async Task<List<Juego>> buscarJuegosAsync(string busqueda)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var resp = await clienteHttp.GetAsync($"juegos/buscar?busqueda={busqueda}");
                if (!resp.IsSuccessStatusCode) return new List<Juego>();
                var data = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Juego>>(data);
            }
        }

        private async Task<List<RecomendacionVista>> obtenerRecomendacionesAsync(int idUsuario)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var resp = await clienteHttp.GetAsync($"juegos/recomendaciones/{idUsuario}");
                if (!resp.IsSuccessStatusCode) return new List<RecomendacionVista>();
                var data = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<RecomendacionVista>>(data);
            }
        }

        private async Task<bool> insertarJuegoAsync(JuegoRegistroDto juego)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var contenido = new StringContent(JsonConvert.SerializeObject(juego), Encoding.UTF8, "application/json");
                var resp = await clienteHttp.PostAsync("juegos", contenido);
                return resp.IsSuccessStatusCode;
            }
        }

        private async Task<bool> editarJuegoAsync(JuegoRegistroDto juego)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var contenido = new StringContent(JsonConvert.SerializeObject(juego), Encoding.UTF8, "application/json");
                var resp = await clienteHttp.PutAsync("juegos", contenido);
                return resp.IsSuccessStatusCode;
            }
        }

        private async Task<bool> desactivarJuegoAsync(int id)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var resp = await clienteHttp.PutAsync($"juegos/desactivar/{id}", null);
                return resp.IsSuccessStatusCode;
            }
        }

        private async Task<bool> activarJuegoAsync(int id)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var resp = await clienteHttp.PutAsync($"juegos/activar/{id}", null);
                return resp.IsSuccessStatusCode;
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

        private async Task<Juego> obtenerJuegoPorIdAsync(int id)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var resp = await clienteHttp.GetAsync($"juegos/{id}");
                if (!resp.IsSuccessStatusCode) return null;
                var data = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Juego>(data);
            }
        }

        private async Task<List<Categoria>> obtenerCategoriasAsync()
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var resp = await clienteHttp.GetAsync("categoria"); // Endpoint de tu API que devuelve todas las categorías
                if (!resp.IsSuccessStatusCode) return new List<Categoria>();
                var data = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Categoria>>(data);
            }
        }


        //--------------------------------------------------------------
        // ==== ACCIONES DEL CONTROLADOR ====

        //--------------------------------------------------------------
        public async Task<ActionResult> Index(int page = 1, int pageSize = 9) // pageSize = juegos por página
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

            // Obtener juegos según rol
            List<Juego> juegos;
            if (Session["ROL"] != null && Session["ROL"].ToString() == "Admin")
                juegos = await obtenerTodosLosJuegosAsync();
            else
                juegos = await obtenerJuegosActivosAsync();

            // Paginación
            int totalJuegos = juegos.Count;
            var juegosPaginados = juegos.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalJuegos / pageSize);

            return View(juegosPaginados);
        }




        public async Task<ActionResult> Buscar(string busqueda)
        {
            var juegos = await buscarJuegosAsync(busqueda);
            return View(juegos);
        }

        public async Task<ActionResult> Recomendaciones()
        {
            if (Session["ID_USUARIO"] == null)
                return RedirectToAction("Login", "Usuario");

            int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
            var recomendaciones = await obtenerRecomendacionesAsync(idUsuario);

            return View(recomendaciones);
        }

        [HttpGet]
        public async Task<ActionResult> InsertarJuego()
        {
            var categorias = await obtenerCategoriasAsync();
            ViewBag.Categorias = new SelectList(categorias, "ID_CATEGORIA", "NOMBRE");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> InsertarJuego(JuegoRegistroDto juego)
        {
            bool ok = await insertarJuegoAsync(juego);
            TempData["mensaje"] = ok
                ? "Juego insertado correctamente."
                : "Error al insertar juego.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> EditarJuego(int id)
        {
            var juegos = await obtenerTodosLosJuegosAsync();
            var juego = juegos.Find(j => j.ID_JUEGO == id);

            var categorias = await obtenerCategoriasAsync();
            ViewBag.Categorias = new SelectList(categorias, "ID_CATEGORIA", "NOMBRE", juego.ID_CATEGORIA);

            return View(juego);
        }

        [HttpPost]
        public async Task<ActionResult> EditarJuego(JuegoRegistroDto juego)
        {
            bool ok = await editarJuegoAsync(juego);
            TempData["mensaje"] = ok
                ? "Juego editado correctamente."
                : "Error al editar juego.";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> DesactivarJuego(int id)
        {
            await desactivarJuegoAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ActivarJuego(int id)
        {
            await activarJuegoAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Detalles(int id)
        {
            var juego = await obtenerJuegoPorIdAsync(id);

            if (juego == null)
                return HttpNotFound();

            if (Session["ID_USUARIO"] != null)
            {
                int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
                await insertarRecomendacionAsync(idUsuario, juego.ID_JUEGO, "Visto recientemente");
            }

            return View(juego);
        }


    }
}
