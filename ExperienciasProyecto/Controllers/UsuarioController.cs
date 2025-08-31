using ExperienciasProyecto.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEB_API_JUEGOS.Models.Dto;

namespace ExperienciasProyecto.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly string apiUrl = ConfigurationManager.AppSettings["URL_API"];

        private async Task<Usuario> loginUsuarioAsync(Usuario request)
        {
            Usuario usuario = null;
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var contenido = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                var mensaje = await clienteHttp.PostAsync("usuario/login", contenido);
                if (mensaje.IsSuccessStatusCode)
                {
                    var data = await mensaje.Content.ReadAsStringAsync();
                    usuario = JsonConvert.DeserializeObject<Usuario>(data);
                }
            }
            return usuario;
        }

        private async Task<PerfilViewModel> obtenerPerfilAsync(int idUsuario)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var mensaje = await clienteHttp.GetAsync($"usuario/{idUsuario}/perfil");
                if (!mensaje.IsSuccessStatusCode)
                    return null;

                var data = await mensaje.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PerfilViewModel>(data);
            }
        }

        private async Task<bool> registrarUsuarioAsync(UsuarioRegistroDto usuario)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                // Ignorar valores nulos al serializar
                var json = JsonConvert.SerializeObject(usuario, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                var mensaje = await clienteHttp.PostAsync("usuario/registrar", contenido);
                var respuesta = await mensaje.Content.ReadAsStringAsync();

                Console.WriteLine($"Código: {mensaje.StatusCode}, Respuesta: {respuesta}");

                return mensaje.IsSuccessStatusCode;
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

        private async Task<decimal> obtenerTotalGastadoAsync(int idUsuario)
        {
            using (var clienteHttp = new HttpClient { BaseAddress = new Uri(apiUrl) })
            {
                var mensaje = await clienteHttp.GetAsync($"usuario/{idUsuario}/totalgastado");
                if (!mensaje.IsSuccessStatusCode) return 0;

                var data = await mensaje.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(data);

                return (decimal)json.totalGastado;
            }
        }

        // ==== ACCIONES DEL CONTROLADOR ====

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(Usuario request)
        {
            var usuario = await loginUsuarioAsync(request);
            if (usuario == null)
            {
                ViewBag.Mensaje = "Credenciales inválidas";
                return View();
            }

            Session["ID_USUARIO"] = usuario.ID_USUARIO;
            Session["NOMBRE"] = usuario.NOMBRE;
            Session["ROL"] = usuario.ROL;

            return RedirectToAction("Index", "Juego");
        }

        [HttpGet]
        public ActionResult RegistrarUsu()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RegistrarUsu(UsuarioRegistroDto usuario)
        {
            // Asignamos el rol fijo sin mostrarlo en la vista
            usuario.ROL = "Usuario";  // 👈 o "Cliente", depende de cómo tu API lo espera

            bool registrado = await registrarUsuarioAsync(usuario);

            if (!registrado)
            {
                ViewBag.Mensaje = "❌ No se pudo registrar el usuario.";
                return View(usuario);
            }

            ViewBag.Mensaje = "✅ Usuario registrado correctamente.";
            return RedirectToAction("Login");
        }


        public async Task<ActionResult> Historial()
        {
            int idUsuario = Session["ID_USUARIO"] != null ? (int)Session["ID_USUARIO"] : 0;
            if (idUsuario == 0) return RedirectToAction("Login");

            var historial = await obtenerHistorialAsync(idUsuario);
            return View(historial);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        public async Task<ActionResult> Perfil()
        {
            int idUsuario = Session["ID_USUARIO"] != null ? (int)Session["ID_USUARIO"] : 0;
            if (idUsuario == 0)
                return RedirectToAction("Login");

            var perfil = await obtenerPerfilAsync(idUsuario);
            if (perfil == null)
            {
                TempData["Mensaje"] = "No se pudo cargar el perfil.";
                return RedirectToAction("Index", "Juego");
            }

            return View(perfil);
        }

        public async Task<ActionResult> TotalGastado()
        {
            if (Session["ID_USUARIO"] == null)
                return RedirectToAction("Login");

            int idUsuario = Convert.ToInt32(Session["ID_USUARIO"]);
            var total = await obtenerTotalGastadoAsync(idUsuario);

            ViewBag.TotalGastado = total;
            return View();
        }
    }
}
