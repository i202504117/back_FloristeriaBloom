namespace back_FloristeriaBloom.Models
{
    public class ProductoListadoDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? Color { get; set; }
        public string? ImagenUrl { get; set; }
        public int CategoriaId { get; set; }
        public string? CategoriaNombre { get; set; }
        public int ProveedorId { get; set; }
        public string? ProveedorNombre { get; set; }
    }
}
