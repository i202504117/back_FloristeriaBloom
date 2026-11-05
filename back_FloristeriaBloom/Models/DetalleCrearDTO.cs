namespace back_FloristeriaBloom.Models
{
    public class DetalleCrearDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public List<int>? AdicionalesIds { get; set; }
    }
}
