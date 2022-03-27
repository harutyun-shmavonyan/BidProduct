namespace BidProduct.DAL.Abstract.Cache
{
    public interface IKeyValueCache<TInput, TExternalValue, TKey, TInternalValue>
    {
        ICacheKeyConverter<TInput, TKey> KeyConverter { get; }
        ICacheValueConverter<TExternalValue, TInternalValue> ValueConverter { get; }

        Task<TExternalValue?> GetAsync(TInput input);
        Task CacheAsync(TInput input, TExternalValue externalValue);
        Task InvalidateAsync(TInput input);
        Task InvalidateWithDelayedSupportAsync(TInput input);
    }
}