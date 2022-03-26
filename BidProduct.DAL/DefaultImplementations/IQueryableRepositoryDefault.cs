using BidProduct.DAL.Abstract;
using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract.Filtering;
using BidProduct.DAL.Abstract.Repositories;
using BidProduct.DAL.Models;

namespace BidProduct.DAL.DefaultImplementations
{
    public interface IQueryableRepositoryDefault<TEntity, TId> : IQueryableRepository<TEntity, TId>,
        IEfRepositoryDefault<TEntity> where TEntity : class, IHasId<TId> where TId : struct
    {
        async Task<TEntity?> IQueryableRepository<TEntity, TId>.GetByIdAsync(TId id)
        {
            var entity = await DbSet.SingleOrDefaultAsync(e => e.Equals(id));
            if (entity == null)
                return default;

            Context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        async Task<ICollection<TEntity>?> IQueryableRepository<TEntity, TId>.GetByFilterAsync(
            FilterBase<TEntity, TId>? filter)
        {
            if (filter == null)
                return await DbSet.AsNoTracking().ToListAsync();
            var query = filter.Execute(DbSet, IncludeFilterExecutor, ProjectionFilterExecutor);
            return await query.AsNoTracking().ToListAsync();
        }

        async Task<TEntity> IQueryableRepository<TEntity, TId>.GetSingleByFilterAsync(
            FilterBase<TEntity, TId>? filter)
        {
            if (filter == null)
                return await DbSet.AsNoTracking().SingleAsync();

            var query = filter.Execute(DbSet, IncludeFilterExecutor, ProjectionFilterExecutor);
            return await query.AsNoTracking().SingleAsync();
        }

        async Task<TEntity?> IQueryableRepository<TEntity, TId>.GetSingleOrDefaultByFilterAsync(
            FilterBase<TEntity, TId>? filter)
        {
            if (filter == null)
                return await DbSet.AsNoTracking().SingleOrDefaultAsync();

            IQueryable<TEntity?> query = filter.Execute(DbSet, IncludeFilterExecutor, ProjectionFilterExecutor);
            return await query.AsNoTracking().SingleOrDefaultAsync();
        }

        async Task<long> IQueryableRepository<TEntity, TId>.GetCountByFilterAsync(FilterBase<TEntity, TId>? filter)
        {
            if (filter == null)
                return await DbSet.AsNoTracking().CountAsync();

            filter.IncludedProperties?.Clear();
            var query = filter.Execute(DbSet, IncludeFilterExecutor, ProjectionFilterExecutor);
            return await query.AsNoTracking().CountAsync();
        }
    }
}