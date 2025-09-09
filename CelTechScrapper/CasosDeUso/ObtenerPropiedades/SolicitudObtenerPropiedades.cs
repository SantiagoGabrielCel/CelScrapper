namespace CelTechScrapper.CasosDeUso.ObtenerPropiedades
{
    public class SolicitudObtenerPropiedades
    {
        public string Tipo { get; }
        public string Operacion { get; }
        public string Zona { get; }
        public int MaxPaginas { get; }
        public int? Ambientes { get; }
        public bool IncluirScore { get; }

        public SolicitudObtenerPropiedades(string tipo, string operacion, string zona, int maxPaginas, int? ambientes, bool incluirScore)
        {
            Tipo = tipo;
            Operacion = operacion;
            Zona = zona;
            MaxPaginas = maxPaginas;
            Ambientes = ambientes;
            IncluirScore = incluirScore;
        }
    }
}
