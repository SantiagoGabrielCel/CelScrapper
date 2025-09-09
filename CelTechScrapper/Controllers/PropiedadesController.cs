using Microsoft.AspNetCore.Mvc;
using CelTechScrapper.Aplicacion.CasosDeUso.ObtenerPropiedades;
using CelTechScrapper.Dominio.Entidades;
using CelTechScrapper.CasosDeUso.ObtenerPropiedades;

namespace WebApi.Controladores;

[ApiController]
[Route("api/[controller]")]
public class PropiedadesController : ControllerBase
{
    private readonly ManejadorObtenerPropiedades _manejador;

    public PropiedadesController(ManejadorObtenerPropiedades manejador)
    {
        _manejador = manejador;
    }


    [HttpGet]
    public async Task<ActionResult<List<Propiedad>>> Obtener(
    [FromQuery] string tipo,
    [FromQuery] string operacion,
    [FromQuery] string zona,
    [FromQuery] int maxPaginas,
    [FromQuery] int? ambientes,
    [FromQuery] bool incluirScore = false 
)
    {
        if (string.IsNullOrWhiteSpace(tipo) || string.IsNullOrWhiteSpace(operacion) || string.IsNullOrWhiteSpace(zona) || maxPaginas <= 0)
        {
            return BadRequest("Todos los parámetros son requeridos y deben ser válidos.");
        }

        var solicitud = new SolicitudObtenerPropiedades(tipo, operacion, zona, maxPaginas, ambientes, incluirScore);
        var propiedades = await _manejador.EjecutarAsync(solicitud);

        return Ok(propiedades);
    }
}