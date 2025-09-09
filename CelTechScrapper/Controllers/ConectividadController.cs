using Aplicacion.CasosDeUso.CalcularConectividad;
using CelTechScrapper.CasosDeUso.CalcularConectividad;
using Microsoft.AspNetCore.Mvc;

namespace CelTechScrapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConectividadController : ControllerBase
    {
        private readonly ManejadorConectividad _manejador;

        public ConectividadController(ManejadorConectividad manejador)
        {
            _manejador = manejador;
        }

        [HttpGet]
        public async Task<ActionResult<object>> Calcular([FromQuery] string direccion)
        {
            if (string.IsNullOrWhiteSpace(direccion))
                return BadRequest("Debe especificarse una dirección.");

            var resultado = await _manejador.EjecutarAsync(new SolicitudConectividad(direccion));
            if (resultado == null) return NotFound("No se pudo geolocalizar la dirección.");

            return Ok(new
            {
                direccion = resultado.Direccion,
                score = resultado.Score
            });
        }
    }
}
