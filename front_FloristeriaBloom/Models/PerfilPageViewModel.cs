namespace front_FloristeriaBloom.Models
{
    public class PerfilPageViewModel
    {
        public PerfilViewModel Perfil { get; set; } = new();
        public List<PedidoListadoViewModel> Pedidos { get; set; } = new();
    }
}
