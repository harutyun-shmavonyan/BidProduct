using BidProduct.DAL.Abstract;
using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract.Repositories;
using BidProduct.DAL.Extensions;

namespace BidProduct.DAL.DefaultImplementations
{
    public interface IRemovableRepositoryDefault<TEntity, TId> : IRemovableRepository<TEntity, TId>,
        IEfRepositoryDefault<TEntity> where TEntity : class, IHasId<TId>, new() where TId : struct
    {
        TEntity IRemovableRepository<TEntity, TId>.Remove(TEntity entity)
        {
            Context.TryAttach<TEntity, TId>(entity);
            Context.Entry(entity).State = EntityState.Deleted;
            return entity;
        }

        void IRemovableRepository<TEntity, TId>.RemoveById(TId id)
        {
            var entity = Context.Set<TEntity>().Local.SingleOrDefault(e => e.Id.Equals(id)) ?? new TEntity { Id = id };

            Context.Entry(entity).State = EntityState.Deleted;
        }

        void IRemovableRepository<TEntity, TId>.BulkRemoveByIds(ICollection<TId> ids)
        {
            foreach (var id in ids)
            {
                RemoveById(id);
            }
        }

        ICollection<TEntity> IRemovableRepository<TEntity, TId>.BulkRemove(ICollection<TEntity> entities) =>
            entities.Select(Remove).ToList();
    }
}