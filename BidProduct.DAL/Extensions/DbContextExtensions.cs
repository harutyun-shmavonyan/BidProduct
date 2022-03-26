using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract;

namespace BidProduct.DAL.Extensions
{
    public static class DbContextExtensions
    {
        public static bool TryAttach<TEntity>(this DbContext context, TEntity entity) where TEntity : class, IHasId
        {
            if (!context.Set<TEntity>().Local.All(e => e.Id != entity.Id && e != entity)) return false;

            context.Attach(entity);
            return true;
        }
    }
}