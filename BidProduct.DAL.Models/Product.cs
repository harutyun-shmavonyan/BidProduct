using BidProduct.DAL.Abstract;

namespace BidProduct.DAL.Models;

public record Product : Entity<long>, IHasCreated
{
    public string Name { get; set; }
    public DateTimeOffset Created { get; set; }
}