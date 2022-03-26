using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.Repositories;
using BidProduct.DAL.Models;

namespace BidProduct.DAL.DefaultImplementations
{
    public interface ICreatableRepositoryDefault<TEntity, TId> : ICreatableRepository<TEntity, TId>,
        IEfRepositoryDefault<TEntity> where TEntity : class, IHasId<TId> where TId : struct
    {
        TEntity ICreatableRepository<TEntity, TId>.Add(TEntity entity)
        {
            var utcNow = DateTime.UtcNow;

            if (entity is IHasCreated hasCreated) hasCreated.Created = utcNow;
            if (entity is IHasModified hasModified) hasModified.Modified = utcNow;

            DbSet.Add(entity);
            return entity;
        }

        ICollection<TEntity> ICreatableRepository<TEntity, TId>.BulkAdd(ICollection<TEntity> entities)
        {
            var utcNow = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                if (entity is IHasCreated hasCreated)
                {
                    hasCreated.Created = utcNow;
                }

                entity.Id = default;
            }
            DbSet.AddRange(entities);
            return entities;
        }
    }
}