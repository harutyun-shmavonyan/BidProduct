namespace BidProduct.DAL.Abstract.Repositories
{
    public interface IRemovableRepository<TEntity, TId> where TEntity : IHasId<TId> where TId : struct
    {
        public TEntity Remove(TEntity entity);
        public void RemoveById(TId id);
        public ICollection<TEntity> BulkRemove(ICollection<TEntity> entities);
        public void BulkRemoveByIds(ICollection<TId> ids);
    }
}