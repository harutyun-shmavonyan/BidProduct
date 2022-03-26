using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract;

namespace BidProduct.DAL.Extensions
{
    public static class DbContextExtensions
    {
        public static bool TryAttach<TEntity, TId>(this DbContext context, TEntity entity)
            where TEntity : class, IHasId<TId> 
            where TId : struct
        {
            if (!context.Set<TEntity>().Local.All(e => !e.Id.Equals(entity.Id) && e != entity)) return false;

            context.Attach(entity);
            return true;
        }
    }
}