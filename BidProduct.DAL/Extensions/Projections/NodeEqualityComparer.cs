namespace BidProduct.DAL.Extensions.Projections
{
    internal class NodeEqualityComparer : IEqualityComparer<Node>
    {
        public bool Equals(Node? x, Node? y)
        {
            return y != null && x != null && x.Type == y.Type && x.PropertyName == y.PropertyName;
        }

        public int GetHashCode(Node obj) => HashCode.Combine(obj.Type, obj.PropertyName);
    }
}