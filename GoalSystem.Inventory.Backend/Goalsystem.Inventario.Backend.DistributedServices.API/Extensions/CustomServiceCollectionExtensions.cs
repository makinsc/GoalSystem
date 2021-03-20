using GoalSystem.Inventario.Backend.API.OperationFilters;
using GoalSystem.Inventario.Backend.API.SwaggerExamples;
using GoalSystem.Inventario.Backend.DistributedServices.API.OperationFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Reflection;

namespace GoalSystem.Inventario.Backend.API.Extensions
{
    public static class CustomServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services) =>

            services.AddApiVersioning(
                options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                });

        /// <summary>
        /// Add custom routing settings which determines how URL's are generated.
        /// </summary>
        public static IServiceCollection AddCustomRouting(this IServiceCollection services) =>
            services.AddRouting(
                options =>
                {
                    // All generated URL's should be lower-case.
                    options.LowercaseUrls = false;
                });


      

        /// <summary>
        /// Adds Swagger services and configures the Swagger services.
        /// </summary>
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                var assembly = typeof(Startup).Assembly;
                var assemblyProduct = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
                var assemblyDescription = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

                options.DescribeAllParametersInCamelCase();
                options.EnableAnnotations();

                // Add the XML comment file for this assembly, so it's contents can be displayed.
                options.OperationFilter<ApiVersionOperationFilter>();
                options.OperationFilter<CorrelationIdOperationFilter>();
                options.ExampleFilters();


                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var apiVersionDescription in provider.ApiVersionDescriptions)
                {
                    var info = new OpenApiInfo()
                    {
                        Title = assemblyProduct,
                        Description = apiVersionDescription.IsDeprecated ?
                            $"{assemblyDescription} Esta versión de la API está obsoleta." :
                            assemblyDescription,
                        Version = apiVersionDescription.ApiVersion.ToString(),
                        TermsOfService = new Uri("https://example.com/terms"),
                        Contact = new OpenApiContact
                        {
                            Name = "GoalSystem",
                            Email = "soporte@goalsystem.com",
                            Url = new Uri("https://goalsystem.com/"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Use under LICX",
                            Url = new Uri("https://example.com/license"),
                        }
                    };
                    options.SwaggerDoc(apiVersionDescription.GroupName, info);

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);
                }

            });
            return services;
        }

        public static IServiceCollection AddCustomSwaggerExamples(this IServiceCollection services) =>
            services
                .AddSwaggerExamplesFromAssemblyOf<GetInventarioModelExample>();                
    }
}
