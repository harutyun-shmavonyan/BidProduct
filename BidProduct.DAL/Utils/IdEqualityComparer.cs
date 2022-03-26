using BidProduct.DAL.Abstract;

namespace BidProduct.DAL.Utils
{
    public class IdEqualityComparer : IEqualityComparer<IHasId>
    {
        public bool Equals(IHasId? x, IHasId? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(IHasId obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}