using CelTechScrapper.Dominio.Modelos;

namespace Aplicacion.Servicios;

public interface IOverpassService
{
    Task<int> ObtenerCantidadPOIsAsync(Coordenada coordenada, double radioMetros, List<string> categorias);
}