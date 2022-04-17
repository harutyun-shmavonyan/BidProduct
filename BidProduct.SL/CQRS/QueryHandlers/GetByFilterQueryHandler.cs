using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.Repositories;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Models.CQRS.Queries;
using IMapper = BidProduct.Common.Abstract.IMapper;

namespace BidProduct.SL.CQRS.QueryHandlers;

public class GetByFilterQueryHandler<TEntity, TId, TResponse> : IInternalRequestHandler<GetByFilterQuery<TEntity, TId, TResponse>, ICollection<TResponse>> 
    where TId : struct 
    where TEntity : class, IHasId<TId>
{
    private readonly IQueryableRepository<TEntity, TId> _repository;
    private readonly IMapper _mapper;

    public GetByFilterQueryHandler(IQueryableRepository<TEntity, TId> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ICollection<TResponse>> HandleAsync(GetByFilterQuery<TEntity, TId, TResponse> request, CancellationToken ct)
    {
        var entities = await _repository.GetByFilterAsync(request.Filter);

        return _mapper.MapMany<TEntity, TResponse>(entities);
    }
}