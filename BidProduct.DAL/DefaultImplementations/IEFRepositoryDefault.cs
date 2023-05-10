using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract.FilterExecutors;
using BidProduct.DAL.DB;
using BidProduct.Common.Abstract;

namespace BidProduct.DAL.DefaultImplementations
{
    public interface IEfRepositoryDefault<TEntity> where TEntity : class
    {
        IDateTimeService DateTimeService { get; set; }
        BidProductDbContext Context { get; set; }
        IIncludeFilterExecutor IncludeFilterExecutor { get; set; }
        IProjectionFilterExecutor ProjectionFilterExecutor { get; set; }
        DbSet<TEntity> DbSet { get; set; }
    }
}