using System.Linq.Expressions;

namespace BidProduct.DAL.Abstract.Repositories
{
    public interface IUpsertableRepository<TEntity> where TEntity : IHasId
    {
        public Task<UpsertResult> UpsertAsync(IChangeTracker<TEntity> changeTracker, Expression<Func<TEntity, bool>> keyExpression, Predicate<TEntity> updatePredicate = null);
        public Task<UpsertResult> UpsertAsync(TEntity entity, Expression<Func<TEntity, bool>> keyExpression, Predicate<TEntity>? updatePredicate = null);
    }
}