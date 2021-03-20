using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.InventarioItem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.Domain.Core.Interfaces
{
    public interface IInventarioItemService
    {
        Task<IEnumerable<InventarioItem>> GetAll();
        Task<IEnumerable<InventarioItem>> GetExpired();
        Task<InventarioItem> InsertarItem(InventarioItem item);    
        Task<bool> ActualizarItem(InventarioItem item);
        Task<bool> SacarItem(Guid item);
        Task<bool> SacarItem(InventarioItem item);
    }
}
