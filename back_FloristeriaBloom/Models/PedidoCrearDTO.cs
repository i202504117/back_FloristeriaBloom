namespace back_FloristeriaBloom.Models
{
    public class PedidoCrearDTO
    {
        public DateTime FechaEntrega { get; set; }
        public string? DireccionEntrega { get; set; }
        public List<DetalleCrearDTO> Detalles { get; set; } = new();
        public string? Ocasion { get; set; }
        public string? MensajeTarjeta { get; set; }
        public string? Firma { get; set; }
        public bool Anonimo { get; set; }
    }
}
