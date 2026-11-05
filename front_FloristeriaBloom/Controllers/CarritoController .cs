using front_FloristeriaBloom.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace front_FloristeriaBloom.Controllers
{
    public class CarritoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string SessionKey = "Carrito";

        public CarritoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private CarritoViewModel ObtenerCarrito()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json))
                return new CarritoViewModel();

            return JsonSerializer.Deserialize<CarritoViewModel>(json) ?? new CarritoViewModel();
        }

        private void GuardarCarrito(CarritoViewModel carrito)
        {
            HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(carrito));
        }

        private async Task<List<AdicionalViewModel>> ObtenerAdicionalesAsync()
        {
            var client = _httpClientFactory.CreateClient("FloristeriaApi");
            var response = await client.GetAsync("Floristeria/adicionales");
            if (!response.IsSuccessStatusCode) return new List<AdicionalViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            var lista = JsonSerializer.Deserialize<List<AdicionalViewModel>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            return lista.Where(a => a.Activo).ToList();
        }

        // ================== AJAX / JSON ==================

        // GET: Carrito/Adicionales -> lista de adicionales activos (para pintar el panel)
        [HttpGet]
        public async Task<IActionResult> Adicionales()
        {
            var lista = await ObtenerAdicionalesAsync();
            return Json(lista);
        }

        // POST: Carrito/AgregarAjax  { id, cantidad?, adicionalesIds? }
        [HttpPost]
        public async Task<IActionResult> AgregarAjax([FromBody] AgregarCarritoRequest request)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return Json(new { ok = false, requiereLogin = true, mensaje = "Debes iniciar sesión para agregar productos." });

            var client = _httpClientFactory.CreateClient("FloristeriaApi");
            var response = await client.GetAsync($"Floristeria/productos/{request.Id}");
            if (!response.IsSuccessStatusCode)
                return Json(new { ok = false, mensaje = "El producto no está disponible." });

            var contenido = await response.Content.ReadAsStringAsync();
            var producto = JsonSerializer.Deserialize<ProductoViewModel>(
                contenido, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (producto == null)
                return Json(new { ok = false, mensaje = "El producto no está disponible." });

            var adicionalesElegidos = new List<AdicionalItemViewModel>();
            if (request.AdicionalesIds != null && request.AdicionalesIds.Any())
            {
                var todos = await ObtenerAdicionalesAsync();
                adicionalesElegidos = todos
                    .Where(a => request.AdicionalesIds.Contains(a.Id))
                    .Select(a => new AdicionalItemViewModel { Id = a.Id, Nombre = a.Nombre, Precio = a.Precio })
                    .ToList();
            }

            var carrito = ObtenerCarrito();

            // Si ya existe una línea con el mismo producto y sin adicionales (caso típico del botón COMPRAR
            // del catálogo), solo se aumenta la cantidad. Si trae adicionales nuevos, se crea una línea aparte.
            var lineaExistente = carrito.Items.FirstOrDefault(i =>
                i.ProductoId == producto.Id && !i.Adicionales.Any() && !adicionalesElegidos.Any());

            if (lineaExistente != null)
            {
                lineaExistente.Cantidad += request.Cantidad;
            }
            else
            {
                carrito.Items.Add(new CarritoItemViewModel
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre ?? "",
                    Precio = producto.Precio,
                    Cantidad = request.Cantidad,
                    ImagenUrl = producto.ImagenUrl,
                    Adicionales = adicionalesElegidos
                });
            }

            GuardarCarrito(carrito);

            return Json(new
            {
                ok = true,
                mensaje = $"{producto.Nombre} se agregó al carrito",
                totalItems = carrito.TotalItems
            });
        }

        // POST: Carrito/AgregarAdicionalesAjax  { lineId, adicionalesIds }
        [HttpPost]
        public async Task<IActionResult> AgregarAdicionalesAjax([FromBody] AdicionalesLineaRequest request)
        {
            var carrito = ObtenerCarrito();
            var linea = carrito.Items.FirstOrDefault(i => i.LineId == request.LineId);
            if (linea == null)
                return Json(new { ok = false, mensaje = "No se encontró el producto en el carrito." });

            var todos = await ObtenerAdicionalesAsync();
            linea.Adicionales = todos
                .Where(a => request.AdicionalesIds.Contains(a.Id))
                .Select(a => new AdicionalItemViewModel { Id = a.Id, Nombre = a.Nombre, Precio = a.Precio })
                .ToList();

            GuardarCarrito(carrito);

            return Json(new { ok = true, subtotal = linea.Subtotal, total = carrito.Total });
        }

        // POST: Carrito/EliminarAjax  { lineId }
        [HttpPost]
        public IActionResult EliminarAjax([FromBody] LineaCarritoRequest request)
        {
            var carrito = ObtenerCarrito();
            var linea = carrito.Items.FirstOrDefault(i => i.LineId == request.LineId);
            if (linea != null) carrito.Items.Remove(linea);

            GuardarCarrito(carrito);

            return Json(new
            {
                ok = true,
                totalItems = carrito.TotalItems,
                totalPrecio = carrito.Total
            });
        }

        // POST: Carrito/GuardarMensajeAjax  { ocasion, mensaje, firma, anonimo }
        [HttpPost]
        public IActionResult GuardarMensajeAjax([FromBody] MensajeCarritoRequest request)
        {
            var carrito = ObtenerCarrito();
            carrito.Ocasion = request.Ocasion;
            carrito.MensajeTarjeta = request.Mensaje;
            carrito.Firma = request.Firma;
            carrito.Anonimo = request.Anonimo;

            GuardarCarrito(carrito);

            return Json(new { ok = true });
        }

        // GET: Carrito/ContarItems
        [HttpGet]
        public IActionResult ContarItems()
        {
            var carrito = ObtenerCarrito();
            return Json(new { totalItems = carrito.TotalItems });
        }

        // ================== FLUJO NORMAL ==================

        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Carrito") });

            return View(ObtenerCarrito());
        }

        [HttpPost]
        public async Task<IActionResult> Confirmar(string direccionEntrega, DateTime fechaEntrega)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Carrito") });

            var carrito = ObtenerCarrito();
            if (!carrito.Items.Any())
                return RedirectToAction("Index");

            var dto = new
            {
                fechaEntrega,
                direccionEntrega,
                ocasion = carrito.Ocasion,
                mensajeTarjeta = carrito.MensajeTarjeta,
                firma = carrito.Firma,
                anonimo = carrito.Anonimo,
                Detalles = carrito.Items.Select(i => new
                {
                    productoId = i.ProductoId,
                    cantidad = i.Cantidad,
                    adicionalesIds = i.Adicionales.Select(a => a.Id).ToList()
                })
            };

            var client = _httpClientFactory.CreateClient("FloristeriaApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var contenido = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("Floristeria/pedidos", contenido);

            if (response.IsSuccessStatusCode)
            {
                GuardarCarrito(new CarritoViewModel());
                return View("Confirmacion");
            }

            ViewBag.Error = "No se pudo procesar el pedido. Verifica tu sesión o el stock disponible.";
            return View("Index", carrito);
        }
    }


    
}