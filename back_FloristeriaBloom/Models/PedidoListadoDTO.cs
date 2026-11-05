namespace back_FloristeriaBloom.Models
{
    public class PedidoListadoDTO
    {
        public int Id { get; set; }
        public string? ClienteNombre { get; set; }
        public DateTime FechaPedido { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string? DireccionEntrega { get; set; }
        public string? Estado { get; set; }
        public decimal Total { get; set; }
        public List<DetalleListadoDTO> Detalles { get; set; } = new();
    }
}
