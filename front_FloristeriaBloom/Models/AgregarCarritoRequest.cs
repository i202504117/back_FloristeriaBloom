namespace front_FloristeriaBloom.Models
{
    public class AgregarCarritoRequest
    {
        public int Id { get; set; }
        public int Cantidad { get; set; } = 1;
        public List<int>? AdicionalesIds { get; set; }
    }
}
