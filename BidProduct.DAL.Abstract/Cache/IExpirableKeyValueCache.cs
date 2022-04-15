namespace BidProduct.DAL.Abstract.Cache
{
    public interface IExpirableKeyValueCache<TInput, TExternalValue, TKey, TInternalValue> : IKeyValueCache<TInput, TExternalValue, TKey, TInternalValue>
    {
        TimeSpan DefaultExpiration { get; }
        Task CacheAsync(TInput input, TExternalValue externalValue, TimeSpan expiration);
    }
}
