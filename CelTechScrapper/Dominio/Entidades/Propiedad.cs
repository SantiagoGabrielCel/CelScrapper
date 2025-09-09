using System.Text.Json.Serialization;

namespace CelTechScrapper.Dominio.Entidades
{
    public class Propiedad
    {
        public string Titulo { get; set; }
        public string Precio { get; set; }
        public string Direccion { get; set; }
        public string Descripcion { get; set; }
        public string Url { get; set; }
        public decimal? PrecioDecimal { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? ScoreConectividad { get; set; }
    }
}
