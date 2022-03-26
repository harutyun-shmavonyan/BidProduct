using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract.Filtering;
using BidProduct.DAL.Abstract.Repositories;
using BidProduct.DAL.Models;

namespace BidProduct.DAL.DefaultImplementations
{
    public interface IQueryableRepositoryDefault<TEntity> : IQueryableRepository<TEntity>,
        IEfRepositoryDefault<TEntity> where TEntity : Entity
    {
        async Task<TEntity?> IQueryableRepository<TEntity>.GetByIdAsync(long id)
        {
            var entity = await DbSet.SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null)
                return null;

            Context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        async Task<ICollection<TEntity>?> IQueryableRepository<TEntity>.GetByFilterAsync(
            FilterBase<TEntity>? filter)
        {
            if (filter == null)
                return await DbSet.AsNoTracking().ToListAsync();
            var query = filter.Execute(DbSet, IncludeFilterExecutor, ProjectionFilterExecutor);
            return await query.AsNoTracking().ToListAsync();
        }

        async Task<TEntity> IQueryableRepository<TEntity>.GetSingleByFilterAsync(
            FilterBase<TEntity>? filter)
        {
            if (filter == null)
                return await DbSet.AsNoTracking().SingleAsync();

            var query = filter.Execute(DbSet, IncludeFilterExecutor, ProjectionFilterExecutor);
            return await query.AsNoTracking().SingleAsync();
        }

        async Task<TEntity?> IQueryableRepository<TEntity>.GetSingleOrDefaultByFilterAsync(
            FilterBase<TEntity>? filter)
        {
            if (filter == null)
                return await DbSet.AsNoTracking().SingleOrDefaultAsync();

            IQueryable<TEntity?> query = filter.Execute(DbSet, IncludeFilterExecutor, ProjectionFilterExecutor);
            return await query.AsNoTracking().SingleOrDefaultAsync();
        }

        async Task<long> IQueryableRepository<TEntity>.GetCountByFilterAsync(FilterBase<TEntity>? filter)
        {
            if (filter == null)
                return await DbSet.AsNoTracking().CountAsync();

            filter.IncludedProperties?.Clear();
            var query = filter.Execute(DbSet, IncludeFilterExecutor, ProjectionFilterExecutor);
            return await query.AsNoTracking().CountAsync();
        }
    }
}