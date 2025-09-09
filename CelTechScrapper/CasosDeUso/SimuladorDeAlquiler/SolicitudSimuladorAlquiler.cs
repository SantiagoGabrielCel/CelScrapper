namespace CelTechScrapper.CasosDeUso.SimuladorDeAlquiler
{
    public class SolicitudSimuladorAlquiler
    {
        public decimal IngresoMensual { get; set; }
        public string Tipo { get; set; }
        public string Operacion { get; set; }
        public string Zona { get; set; }
        public int MaxPaginas { get; set; }

        public SolicitudSimuladorAlquiler(decimal ingresoMensual, string tipo, string operacion, string zona, int maxPaginas)
        {
            IngresoMensual = ingresoMensual;
            Tipo = tipo;
            Operacion = operacion;
            Zona = zona;
            MaxPaginas = maxPaginas;
        }
    }
}
