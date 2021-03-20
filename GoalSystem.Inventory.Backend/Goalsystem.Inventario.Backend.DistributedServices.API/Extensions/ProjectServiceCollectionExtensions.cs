using GoalSystem.Inventario.Backend.API.Profiles;
using GoalSystem.Inventario.Backend.Application.Core.Interfaces;
using GoalSystem.Inventario.Backend.Application.Core.Managers;
using GoalSystem.Inventario.Backend.Domain.Core.Interfaces;
using GoalSystem.Inventario.Backend.Domain.Core.Services;
using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Database.Repositories;
using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.InventarioItem;
using Microsoft.Extensions.DependencyInjection;

namespace GoalSystem.Inventario.Backend.API.Extensions
{
    /// <summary>
    /// Metodo de extensión para registrar la inyección de dependencias
    /// </summary>
    public static class ProjectServiceCollectionExtensions
    {
        /// <summary>
        /// Registra las dependencias de la capa de aplicación
        /// </summary>
        /// <param name="services">Descriptor de servicio</param>
        /// <returns>Descriptor de servicio</returns>
        public static IServiceCollection AddProjectManagers(this IServiceCollection services) =>
            services
                .AddScoped<ICorrelationIdAccesor, CorrelationIdAccesor>()
                .AddTransient<DefaultRequestIdMessageHandler>()                
                .AddScoped<IInventarioItemManager, InventarioItemManager>()
                ;

        /// <summary>
        /// Registra las dependencias de la capa de dominio.
        /// </summary>
        /// <param name="services">Descriptor de servicio</param>
        /// <returns>Descriptor de servicio</returns>
        public static IServiceCollection AddProjectServices(this IServiceCollection services) =>
            services
                .AddScoped<IInventarioItemService, InventarioItemService>();            

        /// <summary>
        /// Registra las dependencias de la capa de infraestructura (repositorios de la capa de acceso a base de datos).
        /// </summary>
        /// <param name="services">Descriptor de servicio</param>
        /// <returns>Descriptor de servicio</returns>
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services) =>
            services
                .AddScoped<IDataRepository<InventarioItem>, InventarioItemRepository>();

        
    }
}
