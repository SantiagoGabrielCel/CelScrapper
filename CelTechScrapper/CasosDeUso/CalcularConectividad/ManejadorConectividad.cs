using Aplicacion.Servicios;
using CelTechScrapper.Aplicacion.Servicios.Geolocalizador;
using CelTechScrapper.CasosDeUso.CalcularConectividad;
using CelTechScrapper.Dominio.DTO;
using CelTechScrapper.Dominio.Modelos;


namespace Aplicacion.CasosDeUso.CalcularConectividad;

public class ManejadorConectividad
{
    private readonly IGeolocalizacionService _geo;
    private readonly IConectividadService _conectividad;

    public ManejadorConectividad(IGeolocalizacionService geo, IConectividadService conectividad)
    {
        _geo = geo;
        _conectividad = conectividad;
    }

    public async Task<ResultadoConectividad> EjecutarAsync(SolicitudConectividad solicitud)
    {
        Coordenada? coord = await _geo.ObtenerCoordenadasAsync(solicitud.Direccion);
        if (coord == null) return new ResultadoConectividad(solicitud.Direccion, 0);

        double score = await _conectividad.CalcularScoreAsync(coord);
        return new ResultadoConectividad(solicitud.Direccion, score);
    }
}