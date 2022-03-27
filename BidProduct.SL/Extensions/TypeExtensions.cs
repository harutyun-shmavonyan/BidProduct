using System.Text;

namespace BidProduct.SL.Extensions
{
    public static class TypeExtensions
    {
        public static string GetFullName(this Type genericType)
        {
            if (!genericType.IsGenericType) return genericType.Name;

            var sb = new StringBuilder();

            sb.Append(genericType.Name.Substring(0, genericType.Name.LastIndexOf("`", StringComparison.Ordinal)));
            sb.Append(genericType.GetGenericArguments().Aggregate("<", (aggregate, type) => aggregate + (aggregate == "<" ? "" : ", ") + GetFullName(type)));
            sb.Append(">");

            return sb.ToString();
        }
    }
}