using MediatR;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.Cache;
using BidProduct.SL.Abstract.CQRS;

namespace BidProduct.SL.Proxies.Cache
{
    public class InternalRequestHandlerExistenceCheckingCacheProxy<TRequest, TResponse, TKey, TValue> : IInternalRequestHandler<TRequest, TResponse>
        where TRequest : class, IInternalRequest<TResponse>, IHasId
        where TResponse : class, IExistenceResponse, new()
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;
        private readonly IExistenceCheckingCache<TRequest, TKey> _existenceCheckingCache;

        public InternalRequestHandlerExistenceCheckingCacheProxy(IRequestHandler<TRequest, TResponse> handler,
            IExistenceCheckingCache<TRequest, TKey> existenceCheckingCache)
        {
            _handler = handler;
            _existenceCheckingCache = existenceCheckingCache;
        }

        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct)
        {
            var exists = await _existenceCheckingCache.ExistsAsync(request);
            if (exists)
            {
                return new TResponse { Exists = true };
            }

            var response = await _handler.Handle(request, ct);

            await _existenceCheckingCache.CacheAsync(request);

            return response;
        }
    }
}