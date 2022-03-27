using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.Repositories;

namespace BidProduct.DAL.DefaultImplementations
{
    public interface IUpsertableRepositoryDefault<TEntity, TId> : IUpsertableRepository<TEntity, TId>,
        IEfRepositoryDefault<TEntity> where TEntity : class, IHasId<TId> where TId : struct
    {
        ICreatableRepository<TEntity, TId> CreatableRepository { get; }
        IUpdateableRepository<TEntity, TId> UpdateableRepository { get; }

        async Task<UpsertResult> IUpsertableRepository<TEntity, TId>.UpsertAsync(IChangeTracker<TEntity, TId> changeTracker,
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

        Task<UpsertResult> IUpsertableRepository<TEntity, TId>.UpsertAsync(TEntity entity, Expression<Func<TEntity, bool>> keyExpression,
            Predicate<TEntity>? updatePredicate)
        {
            var changeTracker = new ChangeTracker<TEntity, TId>(entity, TrackMode.Exclude);
            return UpsertAsync(changeTracker, keyExpression);
        }
    }
}