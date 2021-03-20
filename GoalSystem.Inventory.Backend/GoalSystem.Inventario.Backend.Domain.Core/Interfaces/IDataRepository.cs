using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.Domain.Core.Interfaces
{
    public interface IDataRepository<TEntity>
    {
        IUnitOfWork UnitOfWork { get; }
        IQueryable<TEntity> AsQueryable();
        Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAync(Expression<Func<TEntity, bool>> where);
        void Add(TEntity entity);
        void Update( TEntity entity);
        void Delete(TEntity entity);
    }
}
