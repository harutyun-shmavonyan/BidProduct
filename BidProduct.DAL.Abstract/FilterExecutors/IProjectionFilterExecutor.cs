using System.Linq.Expressions;

namespace BidProduct.DAL.Abstract.FilterExecutors
{
    public interface IProjectionFilterExecutor
    {
        IQueryable<T> Execute<T>(IQueryable<T> query, ICollection<Expression<Func<T, object>>> expressions);
    }
}