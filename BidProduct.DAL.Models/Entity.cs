using BidProduct.DAL.Abstract;

namespace BidProduct.DAL.Models
{
    public record Entity<TId> : IHasId<TId> where TId : struct
    {
        public TId Id { get; set; }
    }
}
