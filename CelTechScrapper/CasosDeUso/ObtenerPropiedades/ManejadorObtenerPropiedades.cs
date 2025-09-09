
using Aplicacion.CasosDeUso.CalcularConectividad;
using Aplicacion.Servicios;
using CelTechScrapper.CasosDeUso.CalcularConectividad;
using CelTechScrapper.CasosDeUso.ObtenerPropiedades;
using CelTechScrapper.Dominio.Entidades;
using System.Threading;

namespace CelTechScrapper.Aplicacion.CasosDeUso.ObtenerPropiedades
{
    public class ManejadorObtenerPropiedades
    {
        private readonly IPropiedadScraperService _servicioScraper;
        private readonly ManejadorConectividad _manejadorConectividad;

        public ManejadorObtenerPropiedades(
            IPropiedadScraperService servicioScraper,
            ManejadorConectividad manejadorConectividad)
        {
            _servicioScraper = servicioScraper;
            _manejadorConectividad = manejadorConectividad;
        }

        public async Task<List<Propiedad>> EjecutarAsync(SolicitudObtenerPropiedades solicitud)
        {
            List<Propiedad> propiedades = await _servicioScraper.ObtenerPropiedadesAsync(
                solicitud.Tipo,
                solicitud.Operacion,
                solicitud.Zona.Replace(" ", "-"),
                solicitud.MaxPaginas,
                solicitud.IncluirScore 
            );

            if (solicitud.IncluirScore)
            {
                Console.WriteLine("[DEBUG] Se solicitó cálculo de ScoreConectividad");
                var semaphore = new SemaphoreSlim(5); // throttle
                var tareas = propiedades.Select(async propiedad =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var solicitudConectividad = new SolicitudConectividad(propiedad.Direccion);
                        var resultado = await _manejadorConectividad.EjecutarAsync(solicitudConectividad);
                        propiedad.ScoreConectividad = resultado.Score;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }).ToList();

                await Task.WhenAll(tareas);
            }

            return propiedades
                .OrderByDescending(p => p.ScoreConectividad ?? 0)
                .ToList();
        }

    }
}
