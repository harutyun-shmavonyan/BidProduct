using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.Repositories;
using BidProduct.DAL.Models;

namespace BidProduct.DAL.DefaultImplementations
{
    public interface ICreatableRepositoryDefault<TEntity> : ICreatableRepository<TEntity>,
        IEfRepositoryDefault<TEntity> where TEntity : Entity
    {
        TEntity ICreatableRepository<TEntity>.Add(TEntity entity)
        {
            var utcNow = DateTime.UtcNow;

            if (entity is IHasCreated hasCreated) hasCreated.Created = utcNow;
            if (entity is IHasModified hasModified) hasModified.Modified = utcNow;

            DbSet.Add(entity);
            return entity;
        }

        ICollection<TEntity> ICreatableRepository<TEntity>.BulkAdd(ICollection<TEntity> entities)
        {
            var utcNow = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                if (entity is IHasCreated hasCreated)
                {
                    hasCreated.Created = utcNow;
                }

                entity.Id = 0;
            }
            DbSet.AddRange(entities);
            return entities;
        }
    }
}