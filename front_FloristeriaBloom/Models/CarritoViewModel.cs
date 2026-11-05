namespace front_FloristeriaBloom.Models
{
    public class CarritoViewModel
    {
        public List<CarritoItemViewModel> Items { get; set; } = new();

        // Dedicatoria / mensaje de tarjeta
        public string? Ocasion { get; set; }
        public string? MensajeTarjeta { get; set; }
        public string? Firma { get; set; }
        public bool Anonimo { get; set; }

        public decimal Total => Items.Sum(i => i.Subtotal);
        public int TotalItems => Items.Sum(i => i.Cantidad);
    }
}
