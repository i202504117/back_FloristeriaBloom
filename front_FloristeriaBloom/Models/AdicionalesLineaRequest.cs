namespace front_FloristeriaBloom.Models
{
    public class AdicionalesLineaRequest
    {
        public string LineId { get; set; } = "";
        public List<int> AdicionalesIds { get; set; } = new();
    }
}
