namespace BidProduct.DAL.Abstract.Repositories
{
    public interface IRemovableRepository<TEntity> where TEntity : IHasId
    {
        public TEntity Remove(TEntity entity);
        public void RemoveById(long id);
        public ICollection<TEntity> BulkRemove(ICollection<TEntity> entities);
        public void BulkRemoveByIds(ICollection<long> ids);
    }
}