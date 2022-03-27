using BidProduct.Common.Abstract;

namespace BidProduct.DAL.Abstract.Repositories
{
    public interface ICreatableRepository<TEntity, TId> where TEntity : IHasId<TId> where TId : struct
    {
        public TEntity Add(TEntity entity);
        public ICollection<TEntity> BulkAdd(ICollection<TEntity> entities);
    }
}