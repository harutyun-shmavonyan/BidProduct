using BidProduct.DAL.DefaultImplementations;
using BidProduct.DAL.Models;

namespace BidProduct.DAL.Abstract
{
    public interface IProductRepository : IQueryableRepositoryDefault<Product, long>, ICreatableRepositoryDefault<Product, long>
    {
    }
}
