using BidProduct.DAL.Abstract;

namespace BidProduct.DAL.Utils
{
    public class IdEqualityComparer<TId> : IEqualityComparer<IHasId<TId>> where TId : struct
    {
        public bool Equals(IHasId<TId>? x, IHasId<TId>? y)
        {
            if (ReferenceEquals(x, y)) return true;

            if (x is null) return false;
            if (y is null) return false;

            if (x.GetType() != y.GetType()) return false;
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(IHasId<TId> obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}