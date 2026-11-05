namespace back_FloristeriaBloom.Models
{
    public class DetallePedidoAdicional
    {
        public int Id { get; set; }
        public int DetallePedidoId { get; set; }
        public DetallePedido? DetallePedido { get; set; }

        public int AdicionalId { get; set; }
        public Adicional? Adicional { get; set; }

        public decimal PrecioUnitario { get; set; }
    }
}