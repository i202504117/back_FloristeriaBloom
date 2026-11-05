using front_FloristeriaBloom.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace front_FloristeriaBloom.Controllers
{
    public class PerfilController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PerfilController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool EstaLogueado() => !string.IsNullOrEmpty(HttpContext.Session.GetString("Token"));

        private HttpClient ClienteConToken()
        {
            var client = _httpClientFactory.CreateClient("FloristeriaApi");
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<IActionResult> Index()
        {
            if (!EstaLogueado())
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Perfil") });

            var client = ClienteConToken();

            var perfilResponse = await client.GetAsync("Floristeria/perfil");
            var perfil = new PerfilViewModel();
            if (perfilResponse.IsSuccessStatusCode)
            {
                var json = await perfilResponse.Content.ReadAsStringAsync();
                perfil = JsonSerializer.Deserialize<PerfilViewModel>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            var pedidosResponse = await client.GetAsync("Floristeria/pedidos");
            var pedidos = new List<PedidoListadoViewModel>();
            if (pedidosResponse.IsSuccessStatusCode)
            {
                var json = await pedidosResponse.Content.ReadAsStringAsync();
                pedidos = JsonSerializer.Deserialize<List<PedidoListadoViewModel>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            var vm = new PerfilPageViewModel
            {
                Perfil = perfil,
                Pedidos = pedidos.OrderByDescending(p => p.FechaPedido).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Actualizar(PerfilViewModel model)
        {
            if (!EstaLogueado()) return RedirectToAction("Login", "Account");

            var client = ClienteConToken();
            var json = JsonSerializer.Serialize(new
            {
                nombre = model.Nombre,
                telefono = model.Telefono,
                direccion = model.Direccion
            });

            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("Floristeria/perfil", contenido);

            if (response.IsSuccessStatusCode)
                HttpContext.Session.SetString("Nombre", model.Nombre ?? "");

            TempData["PerfilMensaje"] = response.IsSuccessStatusCode
                ? "Perfil actualizado correctamente."
                : "No se pudo actualizar el perfil.";

            return RedirectToAction("Index");
        }
    }
}