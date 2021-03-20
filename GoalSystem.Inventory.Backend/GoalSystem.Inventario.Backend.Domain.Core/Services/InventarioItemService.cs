using GoalSystem.Inventario.Backend.DistributedServices.SignalR;
using GoalSystem.Inventario.Backend.Domain.Core.Interfaces;
using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.InventarioItem;
using GoalSystem.Inventario.Backend.Transversal.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.Domain.Core.Services
{
    public class InventarioItemService : IInventarioItemService
    {
        private readonly ILogger<InventarioItemService> _logger;
        private readonly IDataRepository<InventarioItem> _inventarioRepository;
        //private readonly IItemDeletedElementEventHandler _deletedEventHandler;
        private readonly IHubContext<ItemInventarioHub> _hub;

        public InventarioItemService(ILogger<InventarioItemService> logger,
            IDataRepository<InventarioItem> inventarioRepository,
            //IItemDeletedElementEventHandler deletedEventHandler,
            IHubContext<ItemInventarioHub> hub)
        {
            _logger = logger;
            _inventarioRepository = inventarioRepository;
            //_deletedEventHandler = deletedEventHandler;
            _hub = hub;
        }

        public async Task<bool> ActualizarItem(InventarioItem item)
        {
            if (item == null || item.Id == Guid.Empty)
            {
                throw new KeyNotFoundException();
            }
            try
            {
                _inventarioRepository.Update(item);
                await _inventarioRepository.UnitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Se ha producido un error actualizando un elemento. item: {JsonConvert.SerializeObject(item)}");
                throw ex;
            }
        }

        public async Task<bool> SacarItem(Guid item)
        {
            if (item == Guid.Empty)
            {
                throw new KeyNotFoundException();
            }
            try
            {
                await Task.Run(() => _inventarioRepository.Delete(new InventarioItem() { Id = item }));
                var result = await _inventarioRepository.UnitOfWork.SaveChangesAsync();
                // mandar vento a todos los clientes de elemento eliminado
                //_deletedEventHandler.Publish(new DeletedMessageReceived() { Id = item.Id, Name = item.Nombre });
                if (result == 1)
                {
                    // mandar vento a todos los clientes de elemento eliminado
                    await _hub.Clients.All.SendAsync("ReceiveDeletedMessage", new DeletedMessageReceived() { Id = item } );
                }
                return true;
            }
            catch ( Exception ex)
            {
                _logger.LogError($"Se ha producido un error sacando un elemento. Item: {JsonConvert.SerializeObject(item)}");
                throw ex;
            }
        }

        public async Task<bool> SacarItem(InventarioItem item)
        {
            if (item == null || item.Id == Guid.Empty)
            {
                throw new KeyNotFoundException();
            }
            try
            {
                await Task.Run(() => _inventarioRepository.Delete(item));
                int result = await _inventarioRepository.UnitOfWork.SaveChangesAsync();                
                // Controlo si el resultado de borrado es satisfactorio.
                //Evitaría mandar más de un mensaje en casos de llamadas concurrentes
                if (result==1)
                {
                    // mandar vento a todos los clientes de elemento eliminado
                    await _hub.Clients.All.SendCoreAsync("ReceiveDeletedMessage", new object[] { new DeletedMessageReceived() { Id = item.Id, Name = item.Nombre } });
                }
                //_deletedEventHandler.Publish(new DeletedMessageReceived() { Id = item.Id, Name = item.Nombre });
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Se ha producido un error sacando un elemento. Item: {JsonConvert.SerializeObject(item)}");
                throw ex;
            }
        }
        
        public async Task<InventarioItem> InsertarItem(InventarioItem item)
        {
            if (item == null || item.Id == Guid.Empty)
            {
                throw new KeyNotFoundException();
            }
            try
            {
                await Task.Run(() => _inventarioRepository.Add(item));
                await _inventarioRepository.UnitOfWork.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Se ha producido un error insertando un elemento. Item: {JsonConvert.SerializeObject(item)}");
                throw ex;
            }
        }

        public async Task<IEnumerable<InventarioItem>> GetAll()
        {
            try
            {
                return await Task.Run(() => _inventarioRepository.AsQueryable()?.ToList());                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Se ha producido un error obteniendo todos los elementos.");
                throw ex;
            }
        }

        public async Task<IEnumerable<InventarioItem>> GetExpired()
        {
            try
            {
                return (await _inventarioRepository.FindByAsync
                        (
                        item => item.FechaCaducidad < DateTime.UtcNow 
                        && !item.IsNotificacionExpiradaEnviada
                        )
                       )?.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Se ha producido un error obteniendo todos los elementos caducados.");
                throw ex;
            }
        }
    }
}
