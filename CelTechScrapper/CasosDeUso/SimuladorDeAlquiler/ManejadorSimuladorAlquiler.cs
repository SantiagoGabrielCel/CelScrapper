using Aplicacion.Servicios;
using CelTechScrapper.Dominio.Entidades;
using CelTechScrapper.DTO;

namespace CelTechScrapper.CasosDeUso.SimuladorDeAlquiler
{
    public class ManejadorSimuladorAlquiler
    {
        private readonly IPropiedadScraperService _scraper;

        public ManejadorSimuladorAlquiler(IPropiedadScraperService scraper)
        {
            _scraper = scraper;
        }

        public async Task<ResultadoSimulador> EjecutarAsync(SolicitudSimuladorAlquiler solicitud)
        {
            string zonaFormateada = solicitud.Zona.Replace(" ", "-");

            decimal minimo = solicitud.IngresoMensual * 0.30m;
            decimal maximo = solicitud.IngresoMensual * 0.40m;

            List<Propiedad> todas = await _scraper.ObtenerPropiedadesAsync(
                solicitud.Tipo,
                solicitud.Operacion,
                zonaFormateada,
                solicitud.MaxPaginas
            );

            List<Propiedad> filtradas = todas
                .Where(p => p.PrecioDecimal.HasValue &&
                            p.PrecioDecimal.Value >= minimo &&
                            p.PrecioDecimal.Value <= maximo)
                .ToList();

            ResultadoSimulador resultado = new ResultadoSimulador
            {
                RangoMinimo = minimo,
                RangoMaximo = maximo,
                PropiedadesRecomendadas = filtradas
            };

            return resultado;
        }
    }
}
