using CelTechScrapper.CasosDeUso.ObtenerIndiceSaturacion;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SaturacionController : ControllerBase
{
    private readonly ManejadorIndiceSaturacion _manejador;

    public SaturacionController(ManejadorIndiceSaturacion manejador)
    {
        _manejador = manejador;
    }

    [HttpGet]
    public async Task<ActionResult<List<SaturacionDTO>>> ObtenerIndice(
        [FromQuery] string zonas,          // ej: "palermo,villa urquiza,recoleta"
        [FromQuery] string tipo,
        [FromQuery] string operacion,
        [FromQuery] int maxPaginas = 3)
    {
        if (string.IsNullOrWhiteSpace(zonas))
            return BadRequest("Debe especificar al menos una zona.");

        List<string> listaZonas = zonas.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                       .Select(z => z.Trim())
                                       .ToList();

        SolicitudIndiceSaturacion solicitud = new SolicitudIndiceSaturacion(
            listaZonas, tipo, operacion, maxPaginas
        );

        List<SaturacionDTO> resultado = await _manejador.EjecutarAsync(solicitud);

        return Ok(resultado);
    }
}
