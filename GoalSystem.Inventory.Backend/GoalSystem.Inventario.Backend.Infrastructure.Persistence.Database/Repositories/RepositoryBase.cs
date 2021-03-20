using GoalSystem.Inventario.Backend.Domain.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.Infrastructure.Persistence.Database.Repositories
{
    public abstract class RepositoryBase<TEntity> : IDataRepository<TEntity> where TEntity:class
    {
        private readonly InventarioContext context;

        public IUnitOfWork UnitOfWork =>  context;

        public RepositoryBase(InventarioContext context){
            this.context = context;
        }

        public virtual void Add(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);           
        }

        public virtual void Delete(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);
        }
        public virtual void Update(TEntity entity)
        {
            context.Set<TEntity>().Update(entity);
        }
        
        public virtual IQueryable<TEntity> AsQueryable()
        {
            return context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await AsQueryable().Where(predicate).ToListAsync();
        }

        public virtual async Task<TEntity> FirstOrDefaultAync(Expression<Func<TEntity, bool>> where)
        {            
            return await AsQueryable().FirstOrDefaultAsync(where);
        }

    }
}
