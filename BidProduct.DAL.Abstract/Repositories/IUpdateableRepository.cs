namespace BidProduct.DAL.Abstract.Repositories
{
    public interface IUpdateableRepository<TEntity, TId> where TEntity : IHasId<TId> where TId : struct
    {
        public TEntity Update(IChangeTracker<TEntity, TId> changeTracker);
        public TEntity Update(TEntity newEntity);
    }
}