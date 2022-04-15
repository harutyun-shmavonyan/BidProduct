using BidProduct.Common;

namespace BidProduct.SL.Models.CQRS.Responses
{
    [Cloneable]
    public record GetProductQueryResponse
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset Created { get; set; }
    }
}