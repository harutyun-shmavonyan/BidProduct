using System.Linq.Expressions;

namespace BidProduct.DAL.Abstract
{
    public interface IChangeTracker<TEntity, TId> where TEntity : IHasId<TId> where TId : struct
    {
        TEntity Entity { get; }
        TrackMode TrackedMode { get; }
        ICollection<Expression<Func<TEntity, object>>> Properties { get; }
        void Register(Expression<Func<TEntity, object>> property);
        void Register(params Expression<Func<TEntity, object>>[] props);
        void Register(ICollection<Expression<Func<TEntity, object>>> props);
    }
}