using AutoMapper;
using GoalSystem.Inventario.Backend.Application.Core.Interfaces;
using GoalSystem.Inventario.Backend.Application.ViewModels.Cliente;
using GoalSystem.Inventario.Backend.Domain.Core.Interfaces;
using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.InventarioItem;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.Application.Core.Managers
{
    public class InventarioItemManager : IInventarioItemManager
    {
        private readonly ILogger<InventarioItemManager> _logger;
        private readonly IMapper _mapper;       
        private readonly IInventarioItemService _inventariosService;

        public InventarioItemManager(ILogger<InventarioItemManager> logger, IMapper mapper,
            IInventarioItemService inventariosService)
        {
            _logger = logger;
            _mapper = mapper;
            _inventariosService = inventariosService;            
        }        

        public async Task<bool> SacarItem(Guid item)
        {            
            try
            {
                return await _inventariosService.SacarItem(item);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error sacando un Item Id {item}.");
                throw ex;
            }
        }

        public async Task<bool> SacarItem(InventarioItemViewModel item)
        {            
            try
            {                
                return await _inventariosService.SacarItem(_mapper.Map<InventarioItem>(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sacando un Item Id {item}.");
                throw ex;
            }
        }

        public Task<bool> ActualizarItem(InventarioItemViewModel item)
        {
            throw new NotImplementedException();
        }
                
        public async Task<InventarioItemViewModel> InsertarItem(InventarioItemViewModel item)
        {            
            try
            {
                await _inventariosService.InsertarItem(_mapper.Map<InventarioItem>(item));
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error insertando un Item Id {item}.");
                throw ex;
            }
        }

        public async Task<IEnumerable<InventarioItemViewModel>> GetAll()
        {            
            try
            {
                return _mapper.Map<IEnumerable<InventarioItemViewModel>>(await _inventariosService.GetAll());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obtenendo todos los elementos");
                throw ex;
            }
        }
    }
}
