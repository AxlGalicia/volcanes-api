using System;
using System.Collections.Generic;

namespace volcanes_api.Models
{
    public partial class Volcan
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public double? Altura { get; set; }
        public string? Ubicacion { get; set; }
        public string? Ecosistema { get; set; }
        public string? Imagen { get; set; }
    }
}
