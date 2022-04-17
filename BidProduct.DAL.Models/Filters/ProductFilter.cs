using BidProduct.DAL.Abstract.FilterExecutors;
using BidProduct.DAL.Abstract.Filtering;

namespace BidProduct.DAL.Models.Filters
{
    public class ProductFilter : FilterBase<Product, long>
    {
        public string? NamePrefix { get; set; }

        public override IQueryable<Product> Execute(IQueryable<Product> query, IIncludeFilterExecutor includeFilterExecutor,
            IProjectionFilterExecutor projectionFilterExecutor)
        {
            if (NamePrefix != null)
                query = query.Where(p => p.Name.StartsWith(NamePrefix));

            return base.Execute(query, includeFilterExecutor, projectionFilterExecutor);
        }
    }
}
