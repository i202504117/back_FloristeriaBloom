using System.ComponentModel.DataAnnotations;

namespace back_FloristeriaBloom.Models
{
    public class Producto
    {
        public int Id { get; set; }
        [Required, MaxLength(150)]
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? ImagenUrl { get; set; }

        [MaxLength(40)]
        public string? Color { get; set; } // NUEVO: Blanco, Rojo, Rosado, Azul, Multicolor...

        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }
    }
}