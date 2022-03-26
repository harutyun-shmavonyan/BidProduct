using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract.Repositories;
using BidProduct.DAL.Extensions;
using BidProduct.DAL.Models;

namespace BidProduct.DAL.DefaultImplementations
{
    public interface IRemovableRepositoryDefault<TEntity> : IRemovableRepository<TEntity>,
        IEfRepositoryDefault<TEntity> where TEntity : Entity, new()
    {
        TEntity IRemovableRepository<TEntity>.Remove(TEntity entity)
        {
            Context.TryAttach(entity);
            Context.Entry(entity).State = EntityState.Deleted;
            return entity;
        }

        void IRemovableRepository<TEntity>.RemoveById(long id)
        {
            var entity = Context.Set<TEntity>().Local.SingleOrDefault(e => e.Id == id) ?? new TEntity { Id = id };

            Context.Entry(entity).State = EntityState.Deleted;
        }

        void IRemovableRepository<TEntity>.BulkRemoveByIds(ICollection<long> ids)
        {
            foreach (var id in ids)
            {
                RemoveById(id);
            }
        }

        ICollection<TEntity> IRemovableRepository<TEntity>.BulkRemove(ICollection<TEntity> entities) =>
            entities.Select(Remove).ToList();
    }
}