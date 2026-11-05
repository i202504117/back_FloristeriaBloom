using front_FloristeriaBloom.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace front_FloristeriaBloom.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public AdminController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
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

        // ===== Listas para los dropdowns de Categoría y Proveedor =====

        private async Task<List<CategoriaViewModel>> ObtenerCategoriasAsync()
        {
            var client = ClienteConToken();
            var response = await client.GetAsync("Floristeria/categorias");
            if (!response.IsSuccessStatusCode) return new List<CategoriaViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CategoriaViewModel>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }

        private async Task<List<ProveedorViewModel>> ObtenerProveedoresAsync()
        {
            var client = ClienteConToken();
            var response = await client.GetAsync("Floristeria/proveedores");
            if (!response.IsSuccessStatusCode) return new List<ProveedorViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProveedorViewModel>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }

        private async Task CargarListasAsync()
        {
            ViewBag.Categorias = await ObtenerCategoriasAsync();
            ViewBag.Proveedores = await ObtenerProveedoresAsync();
        }

        // ===== Subida de imagen: reemplaza el campo de URL manual =====

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

        // GET: Admin/Index -> lista de productos
        public async Task<IActionResult> Index()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            var client = ClienteConToken();
            var response = await client.GetAsync("Floristeria/productos");

            var productos = new List<ProductoViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                productos = JsonSerializer.Deserialize<List<ProductoViewModel>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return View(productos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");
            await CargarListasAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoViewModel model, IFormFile? Imagen)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            var urlImagen = GuardarImagen(Imagen);
            if (urlImagen != null)
                model.ImagenUrl = urlImagen;

            if (string.IsNullOrEmpty(model.ImagenUrl))
                ModelState.AddModelError("", "Debes subir una foto del producto (de frente).");

            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(model);
            }

            var client = ClienteConToken();
            var json = JsonSerializer.Serialize(new
            {
                nombre = model.Nombre,
                descripcion = model.Descripcion,
                precio = model.Precio,
                stock = model.Stock,
                imagenUrl = model.ImagenUrl,
                color = model.Color, // NUEVO
                categoriaId = model.CategoriaId,
                proveedorId = model.ProveedorId
            });

            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("Floristeria/productos", contenido);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "No se pudo crear el producto.");
            await CargarListasAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            var client = ClienteConToken();
            var response = await client.GetAsync($"Floristeria/productos/{id}");
            if (!response.IsSuccessStatusCode) return RedirectToAction("Index");

            var json = await response.Content.ReadAsStringAsync();
            var producto = JsonSerializer.Deserialize<ProductoViewModel>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            await CargarListasAsync();
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductoViewModel model, IFormFile? Imagen)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            // Si no sube una imagen nueva, se conserva la que ya tenía (viene oculta en el form)
            var urlImagen = GuardarImagen(Imagen);
            if (urlImagen != null)
                model.ImagenUrl = urlImagen;

            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(model);
            }

            var client = ClienteConToken();
            var json = JsonSerializer.Serialize(new
            {
                nombre = model.Nombre,
                descripcion = model.Descripcion,
                precio = model.Precio,
                stock = model.Stock,
                imagenUrl = model.ImagenUrl,
                color = model.Color, // NUEVO
                categoriaId = model.CategoriaId,
                proveedorId = model.ProveedorId
            });

            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"Floristeria/productos/{id}", contenido);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "No se pudo actualizar el producto.");
            await CargarListasAsync();
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Account");

            var client = ClienteConToken();
            await client.DeleteAsync($"Floristeria/productos/{id}");
            return RedirectToAction("Index");
        }
    }
}
