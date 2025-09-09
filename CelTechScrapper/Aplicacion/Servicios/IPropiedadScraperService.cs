namespace Aplicacion.Servicios;

using CelTechScrapper.Dominio.Entidades;


public interface IPropiedadScraperService
{
    Task<List<Propiedad>> ObtenerPropiedadesAsync(string tipo, string operacion, string zona, int maxPaginas = 3, bool incluirScore = false);
}