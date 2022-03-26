using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Models.CQRS.Responses;

namespace BidProduct.SL.Models.CQRS.Queries
{
    public record GetProductQuery : IInternalRequest<GetProductQueryResponse>
    {
    }
}
