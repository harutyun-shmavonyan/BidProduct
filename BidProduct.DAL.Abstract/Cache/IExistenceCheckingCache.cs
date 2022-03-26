namespace BidProduct.DAL.Abstract.Cache
{
    public interface IExistenceCheckingCache<TInput, TKey>
        where TInput : class, IHasId
    {
        ICacheKeyConverter<TInput, TKey> KeyConverter { get; }
        Task CacheAsync(TInput input);
        Task<bool> ExistsAsync(TInput input);
        Task RemoveAsync(TInput input, bool needDelayedRemove);
    }
}