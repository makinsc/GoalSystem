using GoalSystem.Inventario.Backend.API.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.API.Controllers
{
    /// <summary>
    /// Estado de salud de la API.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class StatusController : ControllerBase
    {
        private readonly IHealthCheckService _healthCheckService;

        public StatusController(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }


        /// <summary>
        /// Obtiene el status de la API y de sus dependencias, indicando su estado de salud.
        /// </summary>
        /// <returns>OK (200) o la respuesta de error correspondiente indicando el motivo.</returns>
        [HttpGet(Name = StatusControllerRoute.GetStatus)]

        [SwaggerResponse(StatusCodes.Status200OK, "La API funciona normalmente.")]
        [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "La API o alguna de sus dependencias no funciona, servicio no disponible.")]
        public async Task<IActionResult> GetStatus()
        {
            CompositeHealthCheckResult healthCheckResult = await _healthCheckService.CheckHealthAsync();

            bool somethingIsWrong = healthCheckResult.CheckStatus != CheckStatus.Healthy;

            if (somethingIsWrong)
            {
                // healthCheckResult has a .Description property, but that shows the description of all health checks. 
                // Including the successful ones, so let's filter those out
                var failedHealthCheckDescriptions = healthCheckResult.Results.Where(r => r.Value.CheckStatus != CheckStatus.Healthy)
                                                                     .Select(r => r.Value.Description)
                                                                     .ToList();

                // return a 500 with JSON containing the Results of the Health Check
                return new JsonResult(new { Status = healthCheckResult.CheckStatus, Errors = failedHealthCheckDescriptions }) { StatusCode = StatusCodes.Status503ServiceUnavailable };
            }

            return Ok(new { Status = healthCheckResult.CheckStatus });
        }
    }
}