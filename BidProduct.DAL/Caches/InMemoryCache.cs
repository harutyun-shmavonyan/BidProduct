using System.Collections.Concurrent;
using BidProduct.DAL.Abstract.Cache;

namespace BidProduct.DAL.Caches
{
    public class InMemoryCache<TInput, TExternalValue, TKey, TInternalValue> : IKeyValueCache<TInput, TExternalValue, TKey, TInternalValue> where TKey : notnull
    {
        private ConcurrentDictionary<TKey, TInternalValue> _cache = new();

        public ICacheKeyConverter<TInput, TKey> KeyConverter { get; }
        public ICacheValueConverter<TExternalValue, TInternalValue> ValueConverter { get; }

        public InMemoryCache(ICacheKeyConverter<TInput, TKey> keyConverter, ICacheValueConverter<TExternalValue, TInternalValue> valueConverter)
        {
            KeyConverter = keyConverter;
            ValueConverter = valueConverter;
        }

        public Task<TExternalValue?> GetAsync(TInput input)
        {
            var key = KeyConverter.ToKey(input);
            if (_cache.TryGetValue(key, out var internalValue))
            {
                return Task.FromResult(ValueConverter.ConvertToExternalValue(internalValue))!;
            }

            return Task.FromResult((TExternalValue?)(object)null!);
        }

        public Task CacheAsync(TInput input, TExternalValue externalValue)
        {
            var key = KeyConverter.ToKey(input);
            var internalValue = ValueConverter.ConvertToInternalValue(externalValue);
            _cache.TryAdd(key, internalValue);

            return Task.CompletedTask;
        }

        public Task InvalidateAsync(TInput input)
        {
            var key = KeyConverter.ToKey(input);
            _cache.TryRemove(key, out _);

            return Task.CompletedTask;
        }

        public Task InvalidateWithDelayedSupportAsync(TInput input) => InvalidateAsync(input);
    }
}
