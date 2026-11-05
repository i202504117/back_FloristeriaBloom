namespace front_FloristeriaBloom.Models
{
    public class CarritoItemViewModel
    {
       
        public string LineId { get; set; } = Guid.NewGuid().ToString("N");

        public int ProductoId { get; set; }
        public string Nombre { get; set; } = "";
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string? ImagenUrl { get; set; }

        public List<AdicionalItemViewModel> Adicionales { get; set; } = new();

        public decimal PrecioConAdicionales => Precio + Adicionales.Sum(a => a.Precio);
        public decimal Subtotal => PrecioConAdicionales * Cantidad;
    }
}
