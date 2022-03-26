using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract.FilterExecutors;
using BidProduct.DAL.DB;

namespace BidProduct.DAL.DefaultImplementations
{
    public interface IEfRepositoryDefault<TEntity> where TEntity : class
    {
        BidProductDbContext Context { get; set; }
        IIncludeFilterExecutor IncludeFilterExecutor { get; set; }
        IProjectionFilterExecutor ProjectionFilterExecutor { get; set; }
        DbSet<TEntity> DbSet { get; set; }
    }
}