using MediatR;

namespace BidProduct.SL.Abstract.CQRS
{
    public interface IInternalRequestHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IInternalRequest<TResponse>
    {
        public Task<TResponse> HandleAsync(TRequest request, CancellationToken ct);
        Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken) =>
            HandleAsync(request, cancellationToken);
    }

    public interface IInternalRequestHandler<in TRequest> : IInternalRequestHandler<TRequest, object>
        where TRequest : IInternalRequest<object>
    {
    }
}