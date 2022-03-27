using BidProduct.SL.Abstract;
using BidProduct.SL.Abstract.CQRS;
using MediatR;

namespace BidProduct.SL
{
    public class InternalMediator : IInternalMediator
    {
        private readonly IMediator _mediator;

        public InternalMediator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<TResponse> SendAsync<TResponse>(IInternalRequest<TResponse> internalRequest, CancellationToken cancellationToken = default)
            => _mediator.Send(internalRequest, cancellationToken);

        public Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            => _mediator.Publish(notification!, cancellationToken);
    }
}
