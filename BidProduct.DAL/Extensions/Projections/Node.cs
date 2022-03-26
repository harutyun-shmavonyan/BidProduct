using System.Reflection;

namespace BidProduct.DAL.Extensions.Projections
{
    internal class Node
    {
        public string PropertyName { get; init; } = string.Empty;
        public Type? Type { get; init; }
        public bool IsDecoupledCollection { get; init; }

        public HashSet<Node> Children { get; } = new(new NodeEqualityComparer());
        public MemberInfo? MemberInfo { get; init; }

        public override string ToString() => PropertyName;
    }
} 
