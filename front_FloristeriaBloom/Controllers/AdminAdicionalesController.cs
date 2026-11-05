using front_FloristeriaBloom.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace front_FloristeriaBloom.Controllers
{
    public class AdminAdicionalesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public AdminAdicionalesController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _env = env;
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

        private string? GuardarImagen(IFormFile? imagen)
        {
            if (imagen == null || imagen.Length == 0) return null;

            var carpeta = Path.Combine(_env.WebRootPath, "imagenes");
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(imagen.FileName)}";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                imagen.CopyTo(stream);
            }

            return $"/imagenes/{nombreArchivo}";
        }

        // GET: AdminAdicionales/Index -> lista de adicionales
        public async Task<IActionResult> Index()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            var client = ClienteConToken();
            var response = await client.GetAsync("Floristeria/adicionales");

            var adicionales = new List<AdicionalViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                adicionales = JsonSerializer.Deserialize<List<AdicionalViewModel>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return View(adicionales);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");
            return View(new AdicionalViewModel { Activo = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdicionalViewModel model, IFormFile? Imagen)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            var urlImagen = GuardarImagen(Imagen);
            if (urlImagen != null)
                model.ImagenUrl = urlImagen;

            if (!ModelState.IsValid)
                return View(model);

            var client = ClienteConToken();
            var json = JsonSerializer.Serialize(new
            {
                nombre = model.Nombre,
                precio = model.Precio,
                imagenUrl = model.ImagenUrl,
                activo = model.Activo
            });

            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("Floristeria/adicionales", contenido);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "No se pudo crear el adicional.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            var client = ClienteConToken();
            var response = await client.GetAsync($"Floristeria/adicionales/{id}");
            if (!response.IsSuccessStatusCode) return RedirectToAction("Index");

            var json = await response.Content.ReadAsStringAsync();
            var adicional = JsonSerializer.Deserialize<AdicionalViewModel>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(adicional);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AdicionalViewModel model, IFormFile? Imagen)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            // Si no sube una imagen nueva, se conserva la que ya tenía (viene oculta en el form)
            var urlImagen = GuardarImagen(Imagen);
            if (urlImagen != null)
                model.ImagenUrl = urlImagen;

            if (!ModelState.IsValid)
                return View(model);

            var client = ClienteConToken();
            var json = JsonSerializer.Serialize(new
            {
                nombre = model.Nombre,
                precio = model.Precio,
                imagenUrl = model.ImagenUrl,
                activo = model.Activo
            });

            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"Floristeria/adicionales/{id}", contenido);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "No se pudo actualizar el adicional.");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            var client = ClienteConToken();
            await client.DeleteAsync($"Floristeria/adicionales/{id}");
            return RedirectToAction("Index");
        }
    }
}