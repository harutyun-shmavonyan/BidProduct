using BidProduct.SL.Abstract.CQRS;

namespace BidProduct.SL.Abstract
{
    public interface IInternalMediator
    {
        Task<TResponse> SendAsync<TResponse>(IInternalRequest<TResponse> internalRequest,
            CancellationToken cancellationToken = default);
        Task PublishAsync<TNotification>(TNotification notification,
            CancellationToken cancellationToken = default);
    }
}