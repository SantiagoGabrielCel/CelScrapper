namespace CelTechScrapper.CasosDeUso.ObtenerIndiceSaturacion
{
    public class SolicitudIndiceSaturacion
    {
        public List<string> Zonas { get; set; }
        public string Tipo { get; set; }
        public string Operacion { get; set; }
        public int MaxPaginas { get; set; }

        public SolicitudIndiceSaturacion(List<string> zonas, string tipo, string operacion, int maxPaginas = 3)
        {
            Zonas = zonas;
            Tipo = tipo;
            Operacion = operacion;
            MaxPaginas = maxPaginas;
        }
    }
}
