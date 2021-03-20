using AutoMapper;
using CorrelationId;
using GoalSystem.Inventario.Backend.API.Extensions;
using GoalSystem.Inventario.Backend.API.Middleware;
using GoalSystem.Inventario.Backend.DistributedServices.SignalR;
using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Database;
using GoalSystem.Inventario.Backend.Transversal.HostedServices;
using GoalSystem.Inventario.Backend.Transversal.MappingProfile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service collection</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("AlwaysOk", () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Always OK", null)));
            });
            services.AddCorrelationId();
            services.AddMvcCore();
            services.AddVersionedApiExplorer(x => x.GroupNameFormat = "'v'VVV"); // Version format: 'v'major[.minor][-status]
            services.AddCustomApiVersioning();
            services.AddCustomRouting();
            services.AddProjectManagers();
            services.AddProjectServices();
            services.AddRepositoryServices();
            services.AddCustomSwagger(Configuration);
            services.AddCustomSwaggerExamples();        
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddHttpContextAccessor();            
            services.AddCustomDbContext(Configuration);
            services.AddSignalR();
            services.AddHostedService<ExpiredItemHostedService>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Web host environment</param>
        /// <param name="context">Contexto de base de datos</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, InventarioContext context)
        {
            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = "X-Correlation-ID",
                IncludeInResponse = true,
                UpdateTraceIdentifier = true,
                UseGuidForCorrelationId = true
            });
            
            app.UseCorrelationMiddleware();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            if (env.EnvironmentName.ToLower() == "staging")
            {
                app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            }
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
            });
            app.UseCustomSwaggerUI();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ItemInventarioHub>("Hubs/Inventarioitem") ;
            });
            //context.Database.Migrate();
        }
    }
}
