using BidProduct.Common.Abstract;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.FilterExecutors;
using BidProduct.DAL.DB;
using BidProduct.DAL.DefaultImplementations;

namespace BidProduct.DAL.BasicRepositories;

public class RemovableRepository<TEntity, TId> : Repository<TEntity>, IRemovableRepositoryDefault<TEntity, TId>
    where TEntity : class,
    IHasId<TId>, new()
    where TId : struct
{
    public RemovableRepository(BidProductDbContext context, IIncludeFilterExecutor includeFilterExecutor, IProjectionFilterExecutor projectionFilterExecutor, IDateTimeService dateTimeService) : base(context, includeFilterExecutor, projectionFilterExecutor, dateTimeService)
    {
    }
}