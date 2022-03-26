namespace BidProduct.DAL.Abstract.Cache
{
    public interface IExistenceCheckingCache<TInput, TKey, TId>
        where TInput : class, IHasId<TId> where TId : struct
    {
        ICacheKeyConverter<TInput, TKey> KeyConverter { get; }
        Task CacheAsync(TInput input);
        Task<bool> ExistsAsync(TInput input);
        Task RemoveAsync(TInput input, bool needDelayedRemove);
    }
}