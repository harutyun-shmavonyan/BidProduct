using System.Runtime.Caching;
using BidProduct.DAL.Abstract.Cache;

namespace BidProduct.DAL.Caches;

public class InMemoryExpirableCache<TInput, TExternalValue> : IExpirableKeyValueCache<TInput, TExternalValue, string, object>
{
    private readonly MemoryCache _cache = MemoryCache.Default;

    public TimeSpan DefaultExpiration => TimeSpan.FromMinutes(1);

    public ICacheKeyConverter<TInput, string> KeyConverter { get; }
    public ICacheValueConverter<TExternalValue, object> ValueConverter { get; }

    public InMemoryExpirableCache(ICacheKeyConverter<TInput, string> keyConverter, ICacheValueConverter<TExternalValue, object> valueConverter)
    {
        KeyConverter = keyConverter;
        ValueConverter = valueConverter;
    }

    public Task<TExternalValue?> GetAsync(TInput input)
    {
        var key = KeyConverter.ToKey(input);

        var internalValue = _cache.Get(key);

        return internalValue == null ? 
            Task.FromResult((TExternalValue?)(object?)null) :
            Task.FromResult(ValueConverter.ConvertToExternalValue(internalValue));
    }

    public Task CacheAsync(TInput input, TExternalValue externalValue) => CacheAsync(input, externalValue, DefaultExpiration);

    public Task CacheAsync(TInput input, TExternalValue externalValue, TimeSpan expiration)
    {
        var key = KeyConverter.ToKey(input);
        var internalValue = ValueConverter.ConvertToInternalValue(externalValue);

        _cache.Add(key, internalValue, DateTimeOffset.UtcNow.Add(expiration));

        return Task.CompletedTask;
    }

    public Task InvalidateAsync(TInput input)
    {
        var key = KeyConverter.ToKey(input);

        _cache.Remove(key);

        return Task.CompletedTask;
    }

    public Task InvalidateWithDelayedSupportAsync(TInput input) => InvalidateAsync(input);
}