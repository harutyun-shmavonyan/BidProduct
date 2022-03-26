using BidProduct.DAL.Abstract;

namespace BidProduct.DAL.Models
{
    public record Entity : IHasId
    {
        public long Id { get; set; }
    }
}
