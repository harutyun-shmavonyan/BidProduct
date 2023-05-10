using System.Linq.Expressions;
using BidProduct.DAL.Abstract.FilterExecutors;
using Microsoft.EntityFrameworkCore;

namespace BidProduct.DAL.FilterExecutors
{
    public class IncludeFilterExecutor : IIncludeFilterExecutor
    {
        public IQueryable<T> Execute<T>(IQueryable<T> query, ICollection<Expression<Func<T, object>>> expressions)
            where T : class
        {
            return expressions.Aggregate(query, Execute);
        }

        public IQueryable<T> Execute<T>(IQueryable<T> query, Expression<Func<T, object>> exp) where T : class
        {
            return query.Include(exp);
        }
    }
}
