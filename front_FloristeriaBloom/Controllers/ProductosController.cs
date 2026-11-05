using front_FloristeriaBloom.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace front_FloristeriaBloom.Controllers
{
    public class ProductosController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FloristeriaApi");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Floristeria/productos");

            var productos = new List<ProductoViewModel>();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                productos = JsonSerializer.Deserialize<List<ProductoViewModel>>(content, _jsonOptions)
                            ?? new List<ProductoViewModel>();
            }

            return View(productos);
        }
        // NUEVO: página de catálogo con filtros (wireframe)
        public async Task<IActionResult> Catalogo()
        {
            var productos = await ObtenerProductos();
            return View(productos);
        }

        private async Task<List<ProductoViewModel>> ObtenerProductos()
        {
            var response = await _httpClient.GetAsync("Floristeria/productos");
            var productos = new List<ProductoViewModel>();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                productos = JsonSerializer.Deserialize<List<ProductoViewModel>>(content, _jsonOptions)
                            ?? new List<ProductoViewModel>();
            }

            return productos;
        }

    }
}