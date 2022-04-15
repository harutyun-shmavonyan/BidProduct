// ReSharper disable UnusedTypeParameter
namespace BidProduct.DAL.Abstract.Cache;

public interface IExpirationPolicy<TRequest, TResponse>
{
    public TimeSpan Expiration { get; }
}