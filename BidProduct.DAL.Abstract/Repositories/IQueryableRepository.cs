using BidProduct.DAL.Abstract.Filtering;

namespace BidProduct.DAL.Abstract.Repositories
{
    public interface IQueryableRepository<TEntity> where TEntity : class, IHasId
    {
        public Task<TEntity?> GetByIdAsync(long id);
        public Task<long> GetCountByFilterAsync(FilterBase<TEntity>? filter = null);
        public Task<ICollection<TEntity>?> GetByFilterAsync(FilterBase<TEntity>? filter = null);
        public Task<TEntity> GetSingleByFilterAsync(FilterBase<TEntity>? filter = null);
        public Task<TEntity?> GetSingleOrDefaultByFilterAsync(FilterBase<TEntity>? filter = null);
    }
}