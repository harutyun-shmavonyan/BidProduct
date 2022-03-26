using MediatR;
using BidProduct.DAL.Abstract.Cache;
using BidProduct.SL.Abstract.CQRS;

namespace BidProduct.SL.Proxies.Cache
{
    public class InternalRequestHandlerReadThroughCacheProxy<TRequest, TResponse, TKey, TValue> : IInternalRequestHandler<TRequest, TResponse>
        where TRequest : IInternalRequest<TResponse> where TResponse : class
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;
        private readonly IExpirableKeyValueCache<TRequest, TResponse, TKey, TValue> _expirableKeyValueCache;

        public InternalRequestHandlerReadThroughCacheProxy(IRequestHandler<TRequest, TResponse> handler,
            IExpirableKeyValueCache<TRequest, TResponse, TKey, TValue> keyValueCache)
        {
            _handler = handler;
            _expirableKeyValueCache = keyValueCache;
        }

        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct)
        {
            var cacheResult = await _expirableKeyValueCache.GetAsync(request);
            if (cacheResult != null)
            {
                return cacheResult;
            }

            var response = await _handler.Handle(request, ct);

            // ReSharper disable once UnusedVariable
            var cacheMissResolvingTask = _expirableKeyValueCache.DefaultExpirationMilliseconds == null ?
                _expirableKeyValueCache.CacheAsync(request, response) :
                _expirableKeyValueCache.CacheAsync(request, response, _expirableKeyValueCache.DefaultExpirationMilliseconds.Value);

            return response;
        }
    }
}
