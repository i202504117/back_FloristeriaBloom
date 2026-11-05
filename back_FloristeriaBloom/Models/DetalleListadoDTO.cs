namespace back_FloristeriaBloom.Models
{
    public class DetalleListadoDTO
    {
        public string? ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
