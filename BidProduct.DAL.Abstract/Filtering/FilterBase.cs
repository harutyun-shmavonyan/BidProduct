using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using BidProduct.DAL.Abstract.FilterExecutors;

namespace BidProduct.DAL.Abstract.Filtering
{
    public class FilterBase<TEntity, TId> where TEntity : class, IHasId<TId> where TId : struct
    {
        public TId? Id { get; set; }
        public IList<TId>? IdsList { get; set; }

        public int? Take { get; set; }
        public int? Skip { get; set; }

        public IList<OrderedProperty<TEntity>> OrderedProperties { get; } =
            new List<OrderedProperty<TEntity>>();

        public ICollection<Expression<Func<TEntity, object>>>? IncludedProperties { get; } =
            new List<Expression<Func<TEntity, object>>>();

        public ICollection<Expression<Func<TEntity, object>>>? SelectedProperties { get; } =
            new List<Expression<Func<TEntity, object>>>();

        public virtual IQueryable<TEntity> Execute(IQueryable<TEntity> query,
            IIncludeFilterExecutor includeFilterExecutor,
            IProjectionFilterExecutor projectionFilterExecutor)
        {
            if (OrderedProperties.Any())
            {
                var firstOrderingProperty = OrderedProperties[0].Property;
                var orderingQuery = OrderedProperties[0].OrderingForm == OrderingForm.Ascending
                    ? query.OrderBy(firstOrderingProperty)
                    : query.OrderByDescending(firstOrderingProperty);

                for (var i = 1; i < OrderedProperties.Count; ++i)
                {
                    orderingQuery = OrderedProperties[i].OrderingForm == OrderingForm.Ascending
                        ? orderingQuery.ThenBy(OrderedProperties[i].Property)
                        : orderingQuery.ThenByDescending(OrderedProperties[i].Property);
                }

                query = orderingQuery;
            }

            if (Take != null)
                query = query.Take(Take.Value);

            if (Skip != null)
                query = query.Skip(Skip.Value);

            if (Id != null)
                query = query.Where(entity => entity.Equals(Id));

            if (IdsList != null)
                query = query.Where(entity => IdsList.Contains(entity.Id));

            if (IncludedProperties != null && IncludedProperties.Any())
                query = includeFilterExecutor.Execute(query, IncludedProperties);

            if (SelectedProperties != null && SelectedProperties.Any())
                query = projectionFilterExecutor.Execute(query, SelectedProperties);

            return query;
        }
    }
}
