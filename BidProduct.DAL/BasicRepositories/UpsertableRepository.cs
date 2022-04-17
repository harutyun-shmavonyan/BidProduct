using BidProduct.Common.Abstract;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.FilterExecutors;
using BidProduct.DAL.Abstract.Repositories;
using BidProduct.DAL.DB;
using BidProduct.DAL.DefaultImplementations;

namespace BidProduct.DAL.BasicRepositories;

public class UpsertableRepository<TEntity, TId> : Repository<TEntity>, IUpsertableRepositoryDefault<TEntity, TId>
    where TEntity : class,
    IHasId<TId> where TId : struct
{
    public ICreatableRepository<TEntity, TId> CreatableRepository { get; }
    public IUpdateableRepository<TEntity, TId> UpdateableRepository { get; }

    public UpsertableRepository(BidProductDbContext context, IIncludeFilterExecutor includeFilterExecutor, IProjectionFilterExecutor projectionFilterExecutor, IDateTimeService dateTimeService, ICreatableRepository<TEntity, TId> creatableRepository, IUpdateableRepository<TEntity, TId> updateableRepository) : base(context, includeFilterExecutor, projectionFilterExecutor, dateTimeService)
    {
        CreatableRepository = creatableRepository;
        UpdateableRepository = updateableRepository;
    }
}