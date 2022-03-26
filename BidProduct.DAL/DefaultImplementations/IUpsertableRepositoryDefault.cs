using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.Repositories;
using BidProduct.DAL.Models;

namespace BidProduct.DAL.DefaultImplementations
{
    public interface IUpsertableRepositoryDefault<TEntity> : IUpsertableRepository<TEntity>,
        IEfRepositoryDefault<TEntity> where TEntity : Entity
    {
        ICreatableRepository<TEntity> CreatableRepository { get; }
        IUpdateableRepository<TEntity> UpdateableRepository { get; }

        async Task<UpsertResult> IUpsertableRepository<TEntity>.UpsertAsync(IChangeTracker<TEntity> changeTracker,
            Expression<Func<TEntity, bool>> keyExpression,
            Predicate<TEntity>? updatePredicate = null)
        {
            var oldEntity = await DbSet.AsNoTracking().SingleOrDefaultAsync(keyExpression);
            if (oldEntity == null)
            {
                CreatableRepository.Add(changeTracker.Entity);
                return UpsertResult.Inserted;
            }

            changeTracker.Entity.Id = oldEntity.Id;
            if (updatePredicate == null || updatePredicate(oldEntity))
            {
                UpdateableRepository.Update(changeTracker);
                return UpsertResult.Updated;
            }

            return UpsertResult.UnModified;
        }

        Task<UpsertResult> IUpsertableRepository<TEntity>.UpsertAsync(TEntity entity, Expression<Func<TEntity, bool>> keyExpression,
            Predicate<TEntity>? updatePredicate)
        {
            var changeTracker = new ChangeTracker<TEntity>(entity, TrackMode.Exclude);
            return UpsertAsync(changeTracker, keyExpression);
        }
    }
}