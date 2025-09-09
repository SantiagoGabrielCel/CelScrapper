using CelTechScrapper.Dominio.Entidades;

namespace CelTechScrapper.DTO
{
    public class ResultadoSimulador
    {
        public decimal RangoMinimo { get; set; }
        public decimal RangoMaximo { get; set; }
        public List<Propiedad> PropiedadesRecomendadas { get; set; }
    }
}
