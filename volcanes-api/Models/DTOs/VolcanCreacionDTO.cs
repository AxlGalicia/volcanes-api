using System.ComponentModel.DataAnnotations;

namespace volcanes_api.Models.DTOs
{
    public class VolcanCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(maximumLength:255,ErrorMessage = "El campo {0} se excede de {1} caracteres.")]
        public string Nombre { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(maximumLength:65535,ErrorMessage = "El campo {0} se excede de {1} caracteres.")]
        public string Descripcion { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Range(1,8848,ErrorMessage = "El campo {0} deberia de estar en 1 a 8848.")]
        public double Altura { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(maximumLength:255,ErrorMessage = "El campo {0} se excede de {1} caracteres.")]
        public string Ubicacion { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(maximumLength:255,ErrorMessage = "El campo {0} se excede de {1} caracteres.")]
        public string Ecosistema { get; set; }
        
        public IFormFile? Imagen { get; set; }
    }
}
