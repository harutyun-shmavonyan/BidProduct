using BidProduct.Common;
using BidProduct.SL.Extensions;

namespace BidProduct.SL.LogEvents;

// ReSharper disable once UnusedTypeParameter
public record CacheMissEvent<TRequest, TResponse, TKey> : LogEvent
{
    public string RequestType => typeof(TRequest).GetFullName();
    public string CacheSource { get; set; }
    public TKey CacheKey { get; set; }

    public override List<string> Tags { get; set; } = new() {"CacheMiss"};

    public CacheMissEvent(string cacheSource, TKey cacheKey)
    {
        CacheSource = cacheSource;
        CacheKey = cacheKey;
    }
}