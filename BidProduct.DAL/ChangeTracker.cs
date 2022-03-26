using System.Linq.Expressions;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Models;

namespace BidProduct.DAL
{
    public class ChangeTracker<TEntity, TId> : IChangeTracker<TEntity, TId> where TEntity : IHasId<TId> where TId : struct
    {
        public TEntity Entity { get; }
        public TrackMode TrackedMode { get; }
        public ICollection<Expression<Func<TEntity, object>>> Properties { get; }

        public ChangeTracker(TEntity entity, TrackMode mode)
        {
            TrackedMode = mode;
            Entity = entity;
            Properties = new List<Expression<Func<TEntity, object>>>();
        }

        public void Register(Expression<Func<TEntity, object>> expression)
        {
            Properties.Add(expression);
        }

        public void Register(params Expression<Func<TEntity, object>>[] props)
        {
            Register(props.ToList());
        }

        public void Register(ICollection<Expression<Func<TEntity, object>>> props)
        {
            foreach (var expression in props)
            {
                Properties.Add(expression);
            }
        }
    }
}
