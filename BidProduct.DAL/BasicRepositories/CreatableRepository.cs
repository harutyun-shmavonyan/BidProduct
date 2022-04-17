using BidProduct.Common.Abstract;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.FilterExecutors;
using BidProduct.DAL.DB;
using BidProduct.DAL.DefaultImplementations;

namespace BidProduct.DAL.BasicRepositories;

public class CreatableRepository<TEntity, TId> : Repository<TEntity>, ICreatableRepositoryDefault<TEntity, TId>
    where TEntity : class,
    IHasId<TId> where TId : struct
{
    public CreatableRepository(BidProductDbContext context, IIncludeFilterExecutor includeFilterExecutor, IProjectionFilterExecutor projectionFilterExecutor, IDateTimeService dateTimeService) : base(context, includeFilterExecutor, projectionFilterExecutor, dateTimeService)
    {
    }
}