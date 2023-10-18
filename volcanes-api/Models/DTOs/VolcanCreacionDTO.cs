namespace volcanes_api.Models.DTOs
{
    public class VolcanCreacionDTO
    {
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
        public double Altura { get; set; }
        public string Ubicacion { get; set; }

        public string Ecosistema { get; set; }

        public IFormFile Imagen { get; set; }
    }
}
