using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using System;
using System.Reflection;

namespace GoalSystem.Inventario.Backend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogAndRun(CreateHostBuilder(args).Build());
        }

        public static int LogAndRun(IHost host)
        {
            Log.Logger = BuildLogger(host);

            try
            {
                Log.Information(string.Format("Starting application at {0}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")));
                host.Run();
                Log.Information("Stopped application");
                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Application terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseHealthChecks("/hc", TimeSpan.FromSeconds(3));
                    webBuilder.UseStartup<Startup>();
                });

        private static Logger BuildLogger(IHost webHost) =>
            new LoggerConfiguration()
                .ReadFrom.Configuration(webHost.Services.GetRequiredService<IConfiguration>())
                .Enrich.WithProperty("Application", GetAssemblyProductName())
                .CreateLogger();

        private static string GetAssemblyProductName() =>
            Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
    }
}
