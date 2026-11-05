using System.ComponentModel.DataAnnotations;

namespace back_FloristeriaBloom.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string? Nombre { get; set; }
        public string? Contacto { get; set; }
        public string? Direccion { get; set; }

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
