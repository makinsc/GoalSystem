using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.InventarioItem;

namespace GoalSystem.Inventario.Backend.Infrastructure.Persistence.Database.Repositories
{
    public class InventarioItemRepository : RepositoryBase<InventarioItem>
    {        
        public InventarioItemRepository(InventarioContext context):base(context)
        {            
        }        
    }
}
