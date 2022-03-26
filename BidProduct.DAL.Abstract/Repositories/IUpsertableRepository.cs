using System.Linq.Expressions;

namespace BidProduct.DAL.Abstract.Repositories
{
    public interface IUpsertableRepository<TEntity, TId> where TEntity : IHasId<TId> where TId : struct
    {
        public Task<UpsertResult> UpsertAsync(IChangeTracker<TEntity, TId> changeTracker, Expression<Func<TEntity, bool>> keyExpression, Predicate<TEntity> updatePredicate = null);
        public Task<UpsertResult> UpsertAsync(TEntity entity, Expression<Func<TEntity, bool>> keyExpression, Predicate<TEntity>? updatePredicate = null);
    }
}