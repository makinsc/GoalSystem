using GoalSystem.Inventario.Backend.Application.ViewModels.Cliente;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.Application.Core.Interfaces
{
    /// <summary>
    /// Manejador de inventario
    /// </summary>
    public interface IInventarioItemManager
    {
        Task<InventarioItemViewModel> InsertarItem(InventarioItemViewModel item);
        Task<bool> ActualizarItem(InventarioItemViewModel item);
        Task<bool> SacarItem(Guid item);
        Task<bool> SacarItem(InventarioItemViewModel item);
        Task<IEnumerable<InventarioItemViewModel>> GetAll();
    }
}
