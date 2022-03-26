using System.Linq.Expressions;
using BidProduct.DAL.Abstract.FilterExecutors;
using BidProduct.DAL.Extensions.Projections;

namespace BidProduct.DAL.FilterExecutors
{
    public class ProjectionFilterExecutor : IProjectionFilterExecutor
    {
        public IQueryable<T> Execute<T>(IQueryable<T> query, ICollection<Expression<Func<T, object>>> expressions)
        {
            var projections = TreeGenerator<T>.Generate(expressions);
            return query.Select(projections);
        }
    }
}