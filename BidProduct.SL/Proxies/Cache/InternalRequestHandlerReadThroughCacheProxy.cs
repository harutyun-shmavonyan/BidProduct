using MediatR;
using BidProduct.DAL.Abstract.Cache;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Extensions;
using BidProduct.SL.LogEvents;
using Microsoft.Extensions.Logging;
using ILogger = BidProduct.SL.Abstract.ILogger;

namespace BidProduct.SL.Proxies.Cache
{
    public class InternalRequestHandlerReadThroughCacheProxy<TRequest, TResponse, TKey, TValue> : IInternalRequestHandler<TRequest, TResponse>
        where TRequest : IInternalRequest<TResponse> where TResponse : class
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;
        private readonly IKeyValueCache<TRequest, TResponse, TKey, TValue> _keyValueCache;
        private readonly IExpirationPolicy<TRequest, TResponse>? _expirationPolicy;
        private readonly ILogger? _logger;

        public InternalRequestHandlerReadThroughCacheProxy(
            IRequestHandler<TRequest, TResponse> handler,
            IKeyValueCache<TRequest, TResponse, TKey, TValue> keyValueCache, 
            IExpirationPolicy<TRequest, TResponse>? expirationPolicy = null,
            ILogger? logger = null)
        {
            _handler = handler;
            _keyValueCache = keyValueCache;
            _expirationPolicy = expirationPolicy;
            _logger = logger;
        }

        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct)
        {
            var cacheResult = await _keyValueCache.GetAsync(request);
            if (cacheResult != null)
            {
                if(_logger != null)
                    LogServedFromCache(request);

                return cacheResult;
            }

            if (_logger != null)
                LogCacheMiss(request);

            var response = await _handler.Handle(request, ct);

            if (_keyValueCache is IExpirableKeyValueCache<TRequest, TResponse, TKey, TValue> expirableKeyValueCache)
            {
                if (_expirationPolicy != null)
                {
#pragma warning disable CS4014
                    expirableKeyValueCache.CacheAsync(request, response, _expirationPolicy.Expiration);
#pragma warning restore CS4014
                }
                else
                {
#pragma warning disable CS4014
                    expirableKeyValueCache.CacheAsync(request, response);
#pragma warning restore CS4014
                }
            }
            else
            {
#pragma warning disable CS4014
                _keyValueCache.CacheAsync(request, response);
#pragma warning restore CS4014
            }
            
            return response;
        }

        private void LogCacheMiss(TRequest internalRequest)
        {
            var cacheSource = _keyValueCache.GetType().GetFullName();
            var cacheKey = _keyValueCache.ToKey(internalRequest);

            _logger?.Log(new CacheMissEvent<TRequest, TResponse, TKey>(cacheSource, cacheKey), LogLevel.Information);
        }

        private void LogServedFromCache(TRequest internalRequest)
        {
            var cacheSource = _keyValueCache.GetType().GetFullName();
            var cacheKey = _keyValueCache.ToKey(internalRequest);

            _logger?.Log(new CacheServeEvent<TRequest, TResponse, TKey>(cacheSource, cacheKey), LogLevel.Information);
        }
    }
}
