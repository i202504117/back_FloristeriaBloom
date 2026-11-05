using System.ComponentModel.DataAnnotations;

namespace back_FloristeriaBloom.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.Now;
        public DateTime FechaEntrega { get; set; }
        [Required]
        public string? DireccionEntrega { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public decimal Total { get; set; }
        public string? Ocasion { get; set; }
        public string? MensajeTarjeta { get; set; }
        public string? Firma { get; set; }
        public bool Anonimo { get; set; }

        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
