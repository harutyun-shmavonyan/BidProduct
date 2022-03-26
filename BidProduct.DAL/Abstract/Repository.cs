using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract.FilterExecutors;
using BidProduct.DAL.DB;
using BidProduct.DAL.DefaultImplementations;

namespace BidProduct.DAL.Abstract
{
    public class Repository<TEntity> : IEfRepositoryDefault<TEntity> where TEntity : class
    {
        public BidProductDbContext Context { get; set; }
        public IIncludeFilterExecutor IncludeFilterExecutor { get; set; }
        public IProjectionFilterExecutor ProjectionFilterExecutor { get; set; }
        public DbSet<TEntity> DbSet { get; set; }

        public Repository(BidProductDbContext context,
            IIncludeFilterExecutor includeFilterExecutor,
            IProjectionFilterExecutor projectionFilterExecutor)
        {
            Context = context;
            IncludeFilterExecutor = includeFilterExecutor;
            ProjectionFilterExecutor = projectionFilterExecutor;
            DbSet = context.Set<TEntity>();
        }
    }
}
