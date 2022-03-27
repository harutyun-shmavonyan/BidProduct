using System.Linq.Expressions;

namespace BidProduct.DAL.Abstract.FilterExecutors
{
    public interface IIncludeFilterExecutor
    {
        IQueryable<T> Execute<T>(IQueryable<T> query, ICollection<Expression<Func<T, object>>> expressions)
            where T : class;

        IQueryable<T> Execute<T>(IQueryable<T> query, Expression<Func<T, object>> exp)
            where T : class;
    }
}