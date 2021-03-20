using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace GoalSystem.Inventario.Backend.DistributedServices.API.OperationFilters
{
    public class ApiVersionOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiVersion = context.ApiDescription.GetApiVersion();

            // If the api explorer did not capture an API version for this operation then the action must be API
            // version-neutral, so there's nothing to add.
            if (apiVersion == null)
            {
                return;
            }

            var parameters = operation.Parameters;

            if (parameters == null)
            {
                operation.Parameters = parameters = new List<OpenApiParameter>();
            }

            // Note: Version applied is by adding a query string parameter method with the name "api-version".

            // consider the url path segment parameter first
            var parameter = parameters.FirstOrDefault(p => p.Name == "api-version");
            if (parameter == null)
            {
                // the only other method in this sample is by query string
                parameter = new OpenApiParameter()
                {
                    Name = "api-version",
                    Required = false,
                    In = ParameterLocation.Query
                };
                parameters.Add(parameter);
            }

            parameter.Description = "Versión solicitada de la API";
            parameter.Required = false;
        }
    }
}
