using BidProduct.Common.Abstract;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.FilterExecutors;
using BidProduct.DAL.DB;
using BidProduct.DAL.DefaultImplementations;

namespace BidProduct.DAL.BasicRepositories;

public class UpdateableRepository<TEntity, TId> : Repository<TEntity>, IUpdateableRepositoryDefault<TEntity, TId>
    where TEntity : class,
    IHasId<TId> where TId : struct
{
    public UpdateableRepository(BidProductDbContext context, IIncludeFilterExecutor includeFilterExecutor, IProjectionFilterExecutor projectionFilterExecutor, IDateTimeService dateTimeService) : base(context, includeFilterExecutor, projectionFilterExecutor, dateTimeService)
    {
    }
}