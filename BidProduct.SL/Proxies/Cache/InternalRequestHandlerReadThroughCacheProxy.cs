using MediatR;
using BidProduct.DAL.Abstract.Cache;
using BidProduct.SL.Abstract.CQRS;

namespace BidProduct.SL.Proxies.Cache
{
    public class InternalRequestHandlerReadThroughCacheProxy<TRequest, TResponse, TKey, TValue> : IInternalRequestHandler<TRequest, TResponse>
        where TRequest : IInternalRequest<TResponse> where TResponse : class
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;
        private readonly IKeyValueCache<TRequest, TResponse, TKey, TValue> _keyValueCache;

        public InternalRequestHandlerReadThroughCacheProxy(IRequestHandler<TRequest, TResponse> handler,
            IKeyValueCache<TRequest, TResponse, TKey, TValue> keyValueCache)
        {
            _handler = handler;
            _keyValueCache = keyValueCache;
        }

        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct)
        {
            var cacheResult = await _keyValueCache.GetAsync(request);
            if (cacheResult != null)
            {
                return cacheResult;
            }

            var response = await _handler.Handle(request, ct);

            if (_keyValueCache is IExpirableKeyValueCache<TRequest, TResponse, TKey, TValue> expirableKeyValueCache)
            {
                // ReSharper disable once UnusedVariable
                var cacheMissResolvingTask = expirableKeyValueCache.DefaultExpirationMilliseconds == null ?
                    expirableKeyValueCache.CacheAsync(request, response) :
                    expirableKeyValueCache.CacheAsync(request, response, expirableKeyValueCache.DefaultExpirationMilliseconds.Value);
            }
            else
            {
                _keyValueCache.CacheAsync(request, response);
            }
            
            return response;
        }
    }
}
