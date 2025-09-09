using Aplicacion.Servicios;
using CelTechScrapper.Dominio.Entidades;
namespace CelTechScrapper.CasosDeUso.ObtenerIndiceSaturacion;

public class ManejadorIndiceSaturacion
{
    private readonly IPropiedadScraperService _scraper;

    public ManejadorIndiceSaturacion(IPropiedadScraperService scraper)
    {
        _scraper = scraper;
    }

    public async Task<List<SaturacionDTO>> EjecutarAsync(SolicitudIndiceSaturacion solicitud)
    {
        List<SaturacionDTO> resultados = new List<SaturacionDTO>();

        foreach (string zona in solicitud.Zonas)
        {
            string zonaFormateada = zona.Replace(" ", "-");

            List<Propiedad> propiedades = await _scraper.ObtenerPropiedadesAsync(
                solicitud.Tipo,
                solicitud.Operacion,
                zonaFormateada,
                solicitud.MaxPaginas
            );

            List<decimal> preciosValidos = propiedades
                .Where(p => p.PrecioDecimal.HasValue)
                .Select(p => p.PrecioDecimal.Value)
                .ToList();

            decimal promedio = preciosValidos.Any()
                ? Math.Round(preciosValidos.Average(), 2)
                : 0;

            SaturacionDTO dto = new SaturacionDTO
            {
                Zona = zona,
                CantidadPublicaciones = propiedades.Count,
                PrecioPromedio = promedio
            };

            resultados.Add(dto);
        }

        return resultados;
    }
}

