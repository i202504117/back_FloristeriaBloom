using System.ComponentModel.DataAnnotations;

namespace back_FloristeriaBloom.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        [Required, MaxLength(150)]
        public string? Nombre { get; set; }
        [Required, MaxLength(150)]
        public string? Email { get; set; }
        [Required]
        public string? PasswordHash { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string Rol { get; set; } = "Cliente";
    }
}
