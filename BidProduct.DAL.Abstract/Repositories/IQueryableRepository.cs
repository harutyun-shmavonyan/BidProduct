using BidProduct.DAL.Abstract.Filtering;

namespace BidProduct.DAL.Abstract.Repositories
{
    public interface IQueryableRepository<TEntity, TId> where TEntity : class, IHasId<TId> where TId : struct
    {
        public Task<TEntity?> TryGetByIdAsync(TId id);
        public Task<TEntity> GetByIdAsync(TId id);
        public Task<long> GetCountByFilterAsync(FilterBase<TEntity, TId>? filter = null);
        public Task<ICollection<TEntity>?> GetByFilterAsync(FilterBase<TEntity, TId>? filter = null);
        public Task<TEntity> GetSingleByFilterAsync(FilterBase<TEntity, TId>? filter = null);
        public Task<TEntity?> GetSingleOrDefaultByFilterAsync(FilterBase<TEntity, TId>? filter = null);
    }
}