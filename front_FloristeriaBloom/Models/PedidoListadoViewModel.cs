namespace front_FloristeriaBloom.Models
{
    public class PedidoListadoViewModel
    {
        public int Id { get; set; }
        public string? ClienteNombre { get; set; }
        public DateTime FechaPedido { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string? DireccionEntrega { get; set; }
        public string? Estado { get; set; }
        public decimal Total { get; set; }
        public List<DetalleListadoViewModel> Detalles { get; set; } = new();
    }
}
