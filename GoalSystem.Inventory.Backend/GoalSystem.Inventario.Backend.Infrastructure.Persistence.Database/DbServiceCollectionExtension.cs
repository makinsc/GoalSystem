using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace GoalSystem.Inventario.Backend.Infrastructure.Persistence.Database
{
    [ExcludeFromCodeCoverage]
    public static class DbServiceCollectionExtension
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<InventarioContext>(options => options.UseSqlServer(configuration["ConnectionString:InventarioDB"],
                sqlServerOptionsAction: opt =>
                {
                    opt.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                }));
    }
}
