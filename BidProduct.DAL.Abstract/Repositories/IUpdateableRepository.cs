namespace BidProduct.DAL.Abstract.Repositories
{
    public interface IUpdateableRepository<TEntity> where TEntity : IHasId
    {
        public TEntity Update(IChangeTracker<TEntity> changeTracker);
        public TEntity Update(TEntity newEntity);
    }
}