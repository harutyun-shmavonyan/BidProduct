using MediatR;
using BidProduct.DAL.Abstract.Cache;
using BidProduct.SL.Abstract.CQRS;

namespace BidProduct.SL.Proxies.Cache
{
    public class InternalRequestHandlerCacheInvalidatorProxy<TCommand, TCommandResponse, TRequest, TResponse, TKey, TValue> : IInternalRequestHandler<TCommand, TCommandResponse>
        where TCommand : IInternalRequest<TCommandResponse>
    {
        private readonly IRequestHandler<TCommand, TCommandResponse> _handler;
        private readonly IKeyValueCache<TRequest, TResponse, TKey, TValue> _keyValueCache;
        private readonly ICacheCommandToRequestConverter<TCommand, TRequest> _cacheCommandToRequestConverter;

        public InternalRequestHandlerCacheInvalidatorProxy(IRequestHandler<TCommand, TCommandResponse> handler, 
            IKeyValueCache<TRequest, TResponse, TKey, TValue> keyValueCache, 
            ICacheCommandToRequestConverter<TCommand, TRequest> cacheCommandToRequestConverter)
        {
            _handler = handler;
            _keyValueCache = keyValueCache;
            _cacheCommandToRequestConverter = cacheCommandToRequestConverter;
        }

        public async Task<TCommandResponse> HandleAsync(TCommand command, CancellationToken ct)
        {
            var request = _cacheCommandToRequestConverter.Convert(command);

            await _keyValueCache.InvalidateWithDelayedSupportAsync(request);
            return await _handler.Handle(command, ct);
        }
    }
}