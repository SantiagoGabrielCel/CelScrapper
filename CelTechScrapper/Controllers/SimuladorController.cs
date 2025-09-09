using CelTechScrapper.CasosDeUso.SimuladorDeAlquiler;
using CelTechScrapper.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CelTechScrapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimuladorController : ControllerBase
    {
        private readonly ManejadorSimuladorAlquiler _manejador;

        public SimuladorController(ManejadorSimuladorAlquiler manejador)
        {
            _manejador = manejador;
        }

        [HttpGet]
        public async Task<ActionResult<ResultadoSimulador>> Simular(
            [FromQuery] decimal ingresoMensual,
            [FromQuery] string tipo,
            [FromQuery] string operacion,
            [FromQuery] string zona,
            [FromQuery] int maxPaginas)
        {
            if (ingresoMensual <= 0 || string.IsNullOrWhiteSpace(zona))
            {
                return BadRequest("Debe especificarse un ingreso mensual válido y una zona.");
            }

            SolicitudSimuladorAlquiler solicitud = new SolicitudSimuladorAlquiler(
                ingresoMensual, tipo, operacion, zona, maxPaginas
            );

            ResultadoSimulador resultado = await _manejador.EjecutarAsync(solicitud);

            return Ok(resultado);
        }
    }
}
