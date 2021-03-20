using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Linq;
using System.Reflection;

namespace GoalSystem.Inventario.Backend.API.Extensions
{
    public static partial class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomSwaggerUI(this IApplicationBuilder application) =>
                application.UseSwaggerUI(
                    options =>
                    {
                        // Set the Swagger UI browser document title.
                        options.DocumentTitle = typeof(Startup)
                                .Assembly
                                .GetCustomAttribute<AssemblyProductAttribute>()
                                .Product;

                        // Set the Swagger UI to render at '/'.
                        options.RoutePrefix = "api/swagger";

                        // Show the request duration in Swagger UI.
                        options.DisplayRequestDuration();

                        IApiVersionDescriptionProvider provider = (IApiVersionDescriptionProvider)application.ApplicationServices.GetService(typeof(IApiVersionDescriptionProvider));
                        foreach (var apiVersionDescription in provider
                            .ApiVersionDescriptions
                            .OrderByDescending(x => x.ApiVersion))
                        {
                            options.SwaggerEndpoint(
                                $"/api/swagger/{apiVersionDescription.GroupName}/swagger.json",
                                $"Version {apiVersionDescription.ApiVersion}");
                        }
                    });
    }
}
