using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GoalSystem.Inventario.Backend.API.OperationFilters
{
    public class CorrelationIdOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(
                new OpenApiParameter()
                {
                    Description = @"Usada para identificar unívocamente la petición HTTP. Este id es usado para relacionar 
                                    las peticiones realizadas entre el cliente y servidor así como el resto de llamadas a 
                                    los servicios externosbetween a client and server.",
                    In = ParameterLocation.Header,
                    Name = "X-Correlation-ID",
                    Required = false
                });
        }
    }
}
