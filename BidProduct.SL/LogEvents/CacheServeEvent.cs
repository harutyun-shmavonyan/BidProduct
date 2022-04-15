using BidProduct.Common;
using BidProduct.SL.Extensions;

namespace BidProduct.SL.LogEvents;

// ReSharper disable once UnusedTypeParameter
public record CacheServeEvent<TRequest, TResponse, TKey> : LogEvent
{
    public string RequestType => typeof(TRequest).GetFullName();
    public string CacheSource { get; set; }
    public TKey CacheKey { get; set; }

    public override List<string> Topics { get; set; } = new() { "CacheServe" };

    public CacheServeEvent(string cacheSource, TKey cacheKey)
    {
        CacheSource = cacheSource;
        CacheKey = cacheKey;
    }
}