using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.Filtering;
using BidProduct.SL.Abstract.CQRS;

namespace BidProduct.SL.Models.CQRS.Queries;

public record GetByFilterQuery<TEntity, TId, TResponse> : IInternalRequest<ICollection<TResponse>> 
    where TEntity : class, IHasId<TId> 
    where TId : struct
{
    public FilterBase<TEntity, TId> Filter { get; set; }

    public GetByFilterQuery(FilterBase<TEntity, TId> filter)
    {
        Filter = filter;
    }
}