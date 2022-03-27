namespace BidProduct.DAL.Abstract.Cache
{
    public interface IExpirableKeyValueCache<TInput, TExternalValue, TKey, TInternalValue> : IKeyValueCache<TInput, TExternalValue, TKey, TInternalValue>
    {
        long? DefaultExpirationMilliseconds { get; }
        Task CacheAsync(TInput input, TExternalValue externalValue, long expireMilliseconds);
    }
}
