using MediatR;

namespace BidProduct.SL.Abstract.CQRS
{
    public interface IInternalRequest<out TResponse> : IRequest<TResponse>
    {
    }
}
