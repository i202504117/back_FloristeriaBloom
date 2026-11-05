namespace front_FloristeriaBloom.Models
{
    public class AdicionalViewModel
    {
        public int Id { get; set; }
        public string? Nombre { get; set; } 
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public bool Activo { get; set; } = true;
    }
}
