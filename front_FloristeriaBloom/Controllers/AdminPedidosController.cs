using front_FloristeriaBloom.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace front_FloristeriaBloom.Controllers
{
    public class AdminPedidosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminPedidosController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool EsAdmin() => HttpContext.Session.GetString("Rol") == "Admin";

        private HttpClient ClienteConToken()
        {
            var client = _httpClientFactory.CreateClient("FloristeriaApi");
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // GET: AdminPedidos/Index -> lista todos los pedidos
        public async Task<IActionResult> Index()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            var client = ClienteConToken();
            var response = await client.GetAsync("Floristeria/pedidos");

            var pedidos = new List<PedidoListadoViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                pedidos = JsonSerializer.Deserialize<List<PedidoListadoViewModel>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return View(pedidos.OrderByDescending(p => p.FechaPedido).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            var client = ClienteConToken();
            var json = JsonSerializer.Serialize(nuevoEstado); // el backend espera un string JSON simple: "Preparando"
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PutAsync($"Floristeria/pedidos/{id}/estado", contenido);

            return RedirectToAction("Index");
        }

        // Endpoint liviano para el badge de notificaciones (polling desde JS)
        [HttpGet]
        public async Task<IActionResult> PendientesCount()
        {
            if (!EsAdmin()) return Json(new { count = 0 });

            var client = ClienteConToken();
            var response = await client.GetAsync("Floristeria/pedidos");
            if (!response.IsSuccessStatusCode) return Json(new { count = 0 });

            var json = await response.Content.ReadAsStringAsync();
            var pedidos = JsonSerializer.Deserialize<List<PedidoEstadoDTO>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var count = pedidos.Count(p => p.Estado == "Pendiente");
            return Json(new { count });
        }

        private class PedidoEstadoDTO
        {
            public string? Estado { get; set; }
        }
    }
}