using CelTechScrapper.Dominio.Modelos;

namespace CelTechScrapper.Aplicacion.Servicios.Geolocalizador
{
    public interface IGeolocalizacionService
    {
        Task<Coordenada?> ObtenerCoordenadasAsync(string direccion);

    }
}
