using CelTechScrapper.Dominio.Modelos;


namespace Aplicacion.Servicios;

public interface IConectividadService
{
    Task<double> CalcularScoreAsync(Coordenada coordenada);
}
