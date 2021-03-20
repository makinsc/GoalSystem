using GoalSystem.Inventario.Backend.DistributedServices.SignalR;
using GoalSystem.Inventario.Backend.Domain.Core.Interfaces;
using GoalSystem.Inventario.Backend.Transversal.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace GoalSystem.Inventario.Backend.Transversal.HostedServices
{
    public class ExpiredItemHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<ExpiredItemHostedService> _logger;
        private IInventarioItemService _inventarioItemService;
        private readonly IHubContext<ItemInventarioHub> _hub;
        private readonly IServiceProvider _services;
        private Timer _timer;

        public ExpiredItemHostedService(ILogger<ExpiredItemHostedService> logger,
            IServiceProvider services,
            IHubContext<ItemInventarioHub> hub)
        {
            _logger = logger;            
            _hub = hub;
            _services = services;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio Auto Hospedado de items de inventario caducados:  running.");

            _timer = new Timer(SearchExpired, null, 60000, 60000);

            return Task.CompletedTask;
        }

        private async void SearchExpired(object state)
        {
            using (var scope = _services.CreateScope())
            {
                _inventarioItemService =
                    scope.ServiceProvider
                        .GetRequiredService<IInventarioItemService>();

                var itemsExpired = (await _inventarioItemService.GetExpired()).ToList();
                if (itemsExpired.Any())
                {
                    itemsExpired.ForEach(item =>
                    {
                        var task1 = _hub.Clients.All.SendAsync("ItemExpired", new ExpiredMessageReceived() { Id= item.Id, Name = item.Nombre });
                        item.IsNotificacionExpiradaEnviada = true;
                        var task2 = _inventarioItemService.ActualizarItem(item);
                        Task.WaitAll(task1, task2);
                    }

                    );
                }
            }            
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio Auto Hospedado de items de inventario caducados: stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
