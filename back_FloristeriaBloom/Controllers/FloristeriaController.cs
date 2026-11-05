using back_FloristeriaBloom.Data;
using back_FloristeriaBloom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace back_FloristeriaBloom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FloristeriaController : ControllerBase
    {
        private readonly FloristeriaDbContext _context;

        public FloristeriaController(FloristeriaDbContext context)
        {
            _context = context;
        }

        private async Task<SqlConnection> AbrirConexionAsync()
        {
            var conn = (SqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();
            return conn;
        }

        //CATEGORIAS  (con procedimientos almacenados)

        [HttpGet("categorias")]
        public async Task<IActionResult> ListarCategorias()
        {
            var lista = new List<Categoria>();
            var conn = await AbrirConexionAsync();

            using var cmd = new SqlCommand("sp_Categoria_Listar", conn) { CommandType = CommandType.StoredProcedure };
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new Categoria
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion"))
                });
            }
            return Ok(lista);
        }

        [HttpGet("categorias/{id}")]
        public async Task<IActionResult> ObtenerCategoria(int id)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Categoria_Obtener", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return NotFound();

            var categoria = new Categoria
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion"))
            };
            return Ok(categoria);
        }

        [HttpPost("categorias")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearCategoria(Categoria categoria)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Categoria_Crear", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);

            var outputId = new SqlParameter("@NuevoId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outputId);

            await cmd.ExecuteNonQueryAsync();
            return Ok(new { mensaje = "Categoría creada correctamente", id = (int)outputId.Value });
        }

        [HttpPut("categorias/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarCategoria(int id, Categoria categoria)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Categoria_Actualizar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);

            var filas = await cmd.ExecuteNonQueryAsync();
            if (filas == 0) return NotFound();
            return Ok(new { mensaje = "Categoría actualizada correctamente" });
        }

        [HttpDelete("categorias/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Categoria_Eliminar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            var filas = await cmd.ExecuteNonQueryAsync();
            if (filas == 0) return NotFound();
            return Ok(new { mensaje = "Categoría eliminada correctamente" });
        }

        //PROVEEDORES  (con procedimientos almacenados)

        [HttpGet("proveedores")]
        public async Task<IActionResult> ListarProveedores()
        {
            var lista = new List<Proveedor>();
            var conn = await AbrirConexionAsync();

            using var cmd = new SqlCommand("sp_Proveedor_Listar", conn) { CommandType = CommandType.StoredProcedure };
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new Proveedor
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Contacto = reader.IsDBNull(reader.GetOrdinal("Contacto")) ? null : reader.GetString(reader.GetOrdinal("Contacto")),
                    Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString(reader.GetOrdinal("Direccion"))
                });
            }
            return Ok(lista);
        }

        [HttpGet("proveedores/{id}")]
        public async Task<IActionResult> ObtenerProveedor(int id)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Proveedor_Obtener", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return NotFound();

            var proveedor = new Proveedor
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Contacto = reader.IsDBNull(reader.GetOrdinal("Contacto")) ? null : reader.GetString(reader.GetOrdinal("Contacto")),
                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString(reader.GetOrdinal("Direccion"))
            };
            return Ok(proveedor);
        }

        [HttpPost("proveedores")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearProveedor(Proveedor proveedor)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Proveedor_Crear", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Nombre", proveedor.Nombre);
            cmd.Parameters.AddWithValue("@Contacto", (object?)proveedor.Contacto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Direccion", (object?)proveedor.Direccion ?? DBNull.Value);

            var outputId = new SqlParameter("@NuevoId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outputId);

            await cmd.ExecuteNonQueryAsync();
            return Ok(new { mensaje = "Proveedor creado correctamente", id = (int)outputId.Value });
        }

        [HttpPut("proveedores/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarProveedor(int id, Proveedor proveedor)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Proveedor_Actualizar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Nombre", proveedor.Nombre);
            cmd.Parameters.AddWithValue("@Contacto", (object?)proveedor.Contacto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Direccion", (object?)proveedor.Direccion ?? DBNull.Value);

            var filas = await cmd.ExecuteNonQueryAsync();
            if (filas == 0) return NotFound();
            return Ok(new { mensaje = "Proveedor actualizado correctamente" });
        }

        [HttpDelete("proveedores/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarProveedor(int id)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Proveedor_Eliminar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            var filas = await cmd.ExecuteNonQueryAsync();
            if (filas == 0) return NotFound();
            return Ok(new { mensaje = "Proveedor eliminado correctamente" });
        }

        //PRODUCTOS  (con procedimientos almacenados)

        [HttpGet("productos")]
        public async Task<IActionResult> ListarProductos()
        {
            var lista = new List<ProductoListadoDTO>();
            var conn = await AbrirConexionAsync();

            using var cmd = new SqlCommand("sp_Producto_Listar", conn) { CommandType = CommandType.StoredProcedure };
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new ProductoListadoDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                    Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                    Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                    ImagenUrl = reader.IsDBNull(reader.GetOrdinal("ImagenUrl")) ? null : reader.GetString(reader.GetOrdinal("ImagenUrl")),
                    Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                    CategoriaId = reader.GetInt32(reader.GetOrdinal("CategoriaId")),
                    CategoriaNombre = reader.IsDBNull(reader.GetOrdinal("CategoriaNombre")) ? null : reader.GetString(reader.GetOrdinal("CategoriaNombre")),
                    ProveedorId = reader.GetInt32(reader.GetOrdinal("ProveedorId")),
                    ProveedorNombre = reader.IsDBNull(reader.GetOrdinal("ProveedorNombre")) ? null : reader.GetString(reader.GetOrdinal("ProveedorNombre"))
                });
            }
            return Ok(lista);
        }

        [HttpGet("productos/{id}")]
        public async Task<IActionResult> ObtenerProducto(int id)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Producto_Obtener", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return NotFound();

            var producto = new Producto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                ImagenUrl = reader.IsDBNull(reader.GetOrdinal("ImagenUrl")) ? null : reader.GetString(reader.GetOrdinal("ImagenUrl")),
                Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                CategoriaId = reader.GetInt32(reader.GetOrdinal("CategoriaId")),
                ProveedorId = reader.GetInt32(reader.GetOrdinal("ProveedorId"))
            };
            return Ok(producto);
        }

        [HttpPost("productos")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearProducto(ProductoGuardarDTO dto)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Producto_Crear", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Nombre", dto.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)dto.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Precio", dto.Precio);
            cmd.Parameters.AddWithValue("@Stock", dto.Stock);
            cmd.Parameters.AddWithValue("@ImagenUrl", (object?)dto.ImagenUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Color", (object?)dto.Color ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CategoriaId", dto.CategoriaId);
            cmd.Parameters.AddWithValue("@ProveedorId", dto.ProveedorId);

            var outputId = new SqlParameter("@NuevoId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outputId);

            await cmd.ExecuteNonQueryAsync();
            return Ok(new { mensaje = "Producto creado correctamente", id = (int)outputId.Value });
        }

        [HttpPut("productos/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarProducto(int id, ProductoGuardarDTO dto)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Producto_Actualizar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Nombre", dto.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)dto.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Precio", dto.Precio);
            cmd.Parameters.AddWithValue("@Stock", dto.Stock);
            cmd.Parameters.AddWithValue("@ImagenUrl", (object?)dto.ImagenUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Color", (object?)dto.Color ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CategoriaId", dto.CategoriaId);
            cmd.Parameters.AddWithValue("@ProveedorId", dto.ProveedorId);

            var filas = await cmd.ExecuteNonQueryAsync();
            if (filas == 0) return NotFound();
            return Ok(new { mensaje = "Producto actualizado correctamente" });
        }

        [HttpDelete("productos/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Producto_Eliminar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            var filas = await cmd.ExecuteNonQueryAsync();
            if (filas == 0) return NotFound();
            return Ok(new { mensaje = "Producto eliminado correctamente" });
        }
        // ==================== PERFIL DEL CLIENTE ====================

        [HttpGet("perfil")]
        [Authorize]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var clienteId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null) return NotFound();

            return Ok(new
            {
                cliente.Id,
                cliente.Nombre,
                cliente.Email,
                cliente.Telefono,
                cliente.Direccion
            });
        }

        [HttpPut("perfil")]
        [Authorize]
        public async Task<IActionResult> ActualizarPerfil(PerfilActualizarDTO dto)
        {
            var clienteId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null) return NotFound();

            cliente.Nombre = dto.Nombre;
            cliente.Telefono = dto.Telefono;
            cliente.Direccion = dto.Direccion;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Perfil actualizado correctamente" });
        }

        // ==================== ADICIONALES (NUEVO, con procedimientos almacenados) ====================

        [HttpGet("adicionales")]
        public async Task<IActionResult> ListarAdicionales()
        {
            var lista = new List<Adicional>();
            var conn = await AbrirConexionAsync();

            using var cmd = new SqlCommand("sp_Adicional_Listar", conn) { CommandType = CommandType.StoredProcedure };
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new Adicional
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                    ImagenUrl = reader.IsDBNull(reader.GetOrdinal("ImagenUrl")) ? null : reader.GetString(reader.GetOrdinal("ImagenUrl")),
                    Activo = reader.GetBoolean(reader.GetOrdinal("Activo"))
                });
            }
            return Ok(lista);
        }

        [HttpGet("adicionales/{id}")]
        public async Task<IActionResult> ObtenerAdicional(int id)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Adicional_Obtener", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return NotFound();

            var adicional = new Adicional
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                ImagenUrl = reader.IsDBNull(reader.GetOrdinal("ImagenUrl")) ? null : reader.GetString(reader.GetOrdinal("ImagenUrl")),
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo"))
            };
            return Ok(adicional);
        }

        [HttpPost("adicionales")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearAdicional(Adicional adicional)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Adicional_Crear", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Nombre", adicional.Nombre);
            cmd.Parameters.AddWithValue("@Precio", adicional.Precio);
            cmd.Parameters.AddWithValue("@ImagenUrl", (object?)adicional.ImagenUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Activo", adicional.Activo);

            var outputId = new SqlParameter("@NuevoId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outputId);

            await cmd.ExecuteNonQueryAsync();
            return Ok(new { mensaje = "Adicional creado correctamente", id = (int)outputId.Value });
        }

        [HttpPut("adicionales/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarAdicional(int id, Adicional adicional)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Adicional_Actualizar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Nombre", adicional.Nombre);
            cmd.Parameters.AddWithValue("@Precio", adicional.Precio);
            cmd.Parameters.AddWithValue("@ImagenUrl", (object?)adicional.ImagenUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Activo", adicional.Activo);

            var filas = await cmd.ExecuteNonQueryAsync();
            if (filas == 0) return NotFound();
            return Ok(new { mensaje = "Adicional actualizado correctamente" });
        }

        [HttpDelete("adicionales/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarAdicional(int id)
        {
            var conn = await AbrirConexionAsync();
            using var cmd = new SqlCommand("sp_Adicional_Eliminar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            var filas = await cmd.ExecuteNonQueryAsync();
            if (filas == 0) return NotFound();
            return Ok(new { mensaje = "Adicional eliminado correctamente" });
        }

        // ==================== PEDIDOS (Carrito de compras) ====================
        // Ahora acepta adicionales por línea y datos de dedicatoria (Ocasion/Mensaje/Firma/Anonimo)

        [HttpPost("pedidos")]
        [Authorize]
        public async Task<IActionResult> CrearPedido(PedidoCrearDTO dto)
        {
            if (dto.Detalles == null || !dto.Detalles.Any())
                return BadRequest("El carrito está vacío.");

            var clienteIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var clienteId = int.Parse(clienteIdStr!);

            using var transaccion = await _context.Database.BeginTransactionAsync();
            try
            {
                var pedido = new Pedido
                {
                    ClienteId = clienteId,
                    FechaEntrega = dto.FechaEntrega,
                    DireccionEntrega = dto.DireccionEntrega,
                    Estado = "Pendiente",
                    Total = 0,
                    Ocasion = dto.Ocasion,
                    MensajeTarjeta = dto.MensajeTarjeta,
                    Firma = dto.Firma,
                    Anonimo = dto.Anonimo
                };

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync(); // para obtener el Pedido.Id

                decimal totalPedido = 0;

                foreach (var item in dto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if (producto == null)
                        throw new Exception($"El producto {item.ProductoId} no existe.");

                    if (producto.Stock < item.Cantidad)
                        throw new Exception($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}");

                    // Adicionales elegidos para esta línea del carrito
                    var adicionalesSeleccionados = new List<Adicional>();
                    if (item.AdicionalesIds != null && item.AdicionalesIds.Any())
                    {
                        adicionalesSeleccionados = await _context.Adicionales
                            .Where(a => item.AdicionalesIds.Contains(a.Id) && a.Activo)
                            .ToListAsync();
                    }

                    var precioUnitarioConAdicionales = producto.Precio + adicionalesSeleccionados.Sum(a => a.Precio);
                    var subtotal = precioUnitarioConAdicionales * item.Cantidad;

                    var detalle = new DetallePedido
                    {
                        PedidoId = pedido.Id,
                        ProductoId = producto.Id,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = producto.Precio,
                        Subtotal = subtotal
                    };

                    _context.DetallesPedido.Add(detalle);
                    await _context.SaveChangesAsync(); // para obtener el DetallePedido.Id

                    foreach (var adicional in adicionalesSeleccionados)
                    {
                        _context.DetallesPedidoAdicionales.Add(new DetallePedidoAdicional
                        {
                            DetallePedidoId = detalle.Id,
                            AdicionalId = adicional.Id,
                            PrecioUnitario = adicional.Precio
                        });
                    }

                    producto.Stock -= item.Cantidad;
                    totalPedido += subtotal;
                }

                pedido.Total = totalPedido;

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();

                return Ok(new { mensaje = "Pedido creado correctamente", pedidoId = pedido.Id, total = totalPedido });
            }
            catch (Exception ex)
            {
                await transaccion.RollbackAsync();
                return BadRequest(new { mensaje = "No se pudo procesar el pedido.", error = ex.Message });
            }
        }

        [HttpGet("pedidos")]
        [Authorize]
        public async Task<IActionResult> ListarPedidos()
        {
            var esAdmin = User.IsInRole("Admin");
            var clienteId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var query = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .AsQueryable();

            if (!esAdmin)
                query = query.Where(p => p.ClienteId == clienteId);

            var pedidos = await query
                .Select(p => new PedidoListadoDTO
                {
                    Id = p.Id,
                    ClienteNombre = p.Cliente!.Nombre,
                    FechaPedido = p.FechaPedido,
                    FechaEntrega = p.FechaEntrega,
                    DireccionEntrega = p.DireccionEntrega,
                    Estado = p.Estado,
                    Total = p.Total,
                    Detalles = p.Detalles.Select(d => new DetalleListadoDTO
                    {
                        ProductoNombre = d.Producto!.Nombre,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
                })
                .ToListAsync();

            return Ok(pedidos);
        }

        [HttpPut("pedidos/{id}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CambiarEstadoPedido(int id, [FromBody] string nuevoEstado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            pedido.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Estado actualizado correctamente" });
        }

        // ==================== REPORTES ====================

        [HttpGet("reportes/ventas")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReporteVentas(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanoPagina = 10)
        {
            var query = _context.Pedidos.Include(p => p.Cliente).AsQueryable();

            if (fechaInicio.HasValue) query = query.Where(p => p.FechaPedido >= fechaInicio.Value);
            if (fechaFin.HasValue) query = query.Where(p => p.FechaPedido <= fechaFin.Value);

            var total = await query.CountAsync();

            var pedidos = await query
                .OrderByDescending(p => p.FechaPedido)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(p => new { p.Id, ClienteNombre = p.Cliente!.Nombre, p.FechaPedido, p.Estado, p.Total })
                .ToListAsync();

            return Ok(new
            {
                totalRegistros = total,
                pagina,
                tamanoPagina,
                totalPaginas = (int)Math.Ceiling(total / (double)tamanoPagina),
                datos = pedidos
            });
        }

        [HttpGet("reportes/productos-mas-vendidos")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReporteProductosMasVendidos(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            var query = _context.DetallesPedido.Include(d => d.Producto).Include(d => d.Pedido).AsQueryable();

            if (fechaInicio.HasValue) query = query.Where(d => d.Pedido!.FechaPedido >= fechaInicio.Value);
            if (fechaFin.HasValue) query = query.Where(d => d.Pedido!.FechaPedido <= fechaFin.Value);

            var resultado = await query
                .GroupBy(d => new { d.ProductoId, d.Producto!.Nombre })
                .Select(g => new
                {
                    ProductoId = g.Key.ProductoId,
                    ProductoNombre = g.Key.Nombre,
                    CantidadVendida = g.Sum(d => d.Cantidad),
                    TotalVendido = g.Sum(d => d.Subtotal)
                })
                .OrderByDescending(r => r.CantidadVendida)
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("reportes/stock-bajo")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReporteStockBajo([FromQuery] int limite = 10)
        {
            var productos = await _context.Productos
                .Where(p => p.Stock <= limite)
                .OrderBy(p => p.Stock)
                .Select(p => new { p.Id, p.Nombre, p.Stock })
                .ToListAsync();

            return Ok(productos);
        }
    }
}