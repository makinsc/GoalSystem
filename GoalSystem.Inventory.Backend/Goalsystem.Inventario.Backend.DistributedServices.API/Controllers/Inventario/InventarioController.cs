using GoalSystem.Inventario.Backend.Application.Core.Interfaces;
using GoalSystem.Inventario.Backend.Application.ViewModels.Cliente;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.API.Controllers.Inventario
{
    /// <summary>
    /// Controlador de elementos de inventario.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    //[Authorize]        
    public class InventarioController : ControllerBase
    {
        private readonly ILogger<InventarioController> _logger;
        private readonly IInventarioItemManager _inventarioManager;

        /// <summary>
        /// Controlador de inventario.
        /// </summary>
        /// <param name="logger"> Elemento que registrará todas las trazas necesarias para el log de seguimiento.</param>
        /// <param name="inventarioManager">Clase que realizará la lógica de la aplicación del inventario.</param>
        public InventarioController(ILogger<InventarioController> logger, IInventarioItemManager inventarioManager)
        {
            _logger = logger;
            _inventarioManager = inventarioManager;
        }

        /// <summary>
        /// Obtiene la lista de elementos del inventario.
        /// </summary>        
        /// <returns>Listado de todos los items del inventario.</returns>       
        [HttpGet()]
        [Route("/api/Inventario")]
        [SwaggerResponse(StatusCodes.Status200OK, "Operación exitosa", typeof(IEnumerable<InventarioItemViewModel>))]
        //[SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autenticado")]
        //[SwaggerResponse(StatusCodes.Status403Forbidden, "El usuario no tiene permisos para realizar esta acción")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "No hay ningún elemento en el inventario")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ocurrió un error durante la ejecución de la petición")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _inventarioManager.GetAll();
                if (result != null)
                {
                    return Ok(result);
                }
                return NoContent();
            }            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al obtener los elementos del inventario.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Inserta un elemento en el inventario.
        /// </summary>
        /// <param name="item">Elemento que se quiere insertar.</param>
        /// <returns>Devuelve el elemento insertado.</returns>        
        //[HttpPut("{codCliente}")]
        [HttpPut()]
        [Route("/api/Inventario")]
        [SwaggerResponse(StatusCodes.Status200OK, "Operación exitosa", typeof(InventarioItemViewModel))]
        //[SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autenticado")]
        //[SwaggerResponse(StatusCodes.Status403Forbidden, "El usuario no tiene permiso de acceso al cliente solicitado")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "El modelo de entrada no es correcto.")]        
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ocurrió un error durante la ejecución de la petición")]
        
        public async Task<IActionResult> InsertarItem([FromBody]InventarioItemViewModel item)
        {            
            try
            {
                var result = await _inventarioManager.InsertarItem(item);
                if (result != null)
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No se pudo insertar el elemento porque no tiene la clave primaria rellena.{item}");
                return BadRequest();
            }            
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al insertar unnuevo item en el inventario {item}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Saca un elemento de la lista de elementos del inventario.
        /// </summary>
        /// <param name="itemId">Id del elemento que se quiere sacar de la lista</param>        
        /// <returns>Ok si todo fue bien.</returns>
        [HttpDelete()]
        [Route("/api/Inventario/{itemId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Operación exitosa")]
        //[SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuario no autenticado")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Item no encontrado")]
        //[SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(KeyNotFoundErrorResponseModelExample))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ocurrió un error durante la ejecución de la petición")]
        public async Task<IActionResult> SacarItem([FromRoute]Guid itemId)
        {
            try
            {
                var result = await _inventarioManager.SacarItem(itemId);
                if (result)
                {
                    return Ok();
                }
                return NotFound();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"El item indicado no es correcto. itemId = {itemId}");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al sacar el item del inventario {itemId}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}