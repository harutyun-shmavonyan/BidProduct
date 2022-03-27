using MediatR;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.Cache;
using BidProduct.SL.Abstract.CQRS;

namespace BidProduct.SL.Proxies.Cache
{
    public class InternalRequestHandlerExistenceCacheInvalidatorProxy<TCommand, TCommandResponse, TRequest, TKey, TId> : IInternalRequestHandler<TCommand, TCommandResponse>
        where TCommand : IInternalRequest<TCommandResponse> where TRequest : class, IHasId<TId> where TId : struct
    {
        private readonly IRequestHandler<TCommand, TCommandResponse> _handler;
        private readonly IExistenceCheckingCache<TRequest, TKey, TId> _existenceCheckingCache;
        private readonly ICacheCommandToRequestConverter<TCommand, TRequest> _cacheCommandToRequestConverter;

        public InternalRequestHandlerExistenceCacheInvalidatorProxy(IRequestHandler<TCommand, TCommandResponse> handler,
            IExistenceCheckingCache<TRequest, TKey, TId> existenceCheckingCache,
            ICacheCommandToRequestConverter<TCommand, TRequest> cacheCommandToRequestConverter)
        {
            _handler = handler;
            _existenceCheckingCache = existenceCheckingCache;
            _cacheCommandToRequestConverter = cacheCommandToRequestConverter;
        }

        public async Task<TCommandResponse> HandleAsync(TCommand command, CancellationToken ct)
        {
            var request = _cacheCommandToRequestConverter.Convert(command);

            await _existenceCheckingCache.RemoveAsync(request, true);
            return await _handler.Handle(command, ct);
        }
    }
}