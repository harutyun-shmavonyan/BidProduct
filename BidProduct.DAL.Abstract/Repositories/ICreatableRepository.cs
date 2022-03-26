namespace BidProduct.DAL.Abstract.Repositories
{
    public interface ICreatableRepository<TEntity> where TEntity : IHasId
    {
        public TEntity Add(TEntity entity);
        public ICollection<TEntity> BulkAdd(ICollection<TEntity> entities);
    }
}