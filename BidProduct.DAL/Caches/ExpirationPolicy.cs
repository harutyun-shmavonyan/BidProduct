using BidProduct.DAL.Abstract.Cache;
using System;

namespace BidProduct.DAL.Caches;

public class ExpirationPolicy<TRequest, TResponse> : IExpirationPolicy<TRequest, TResponse>
{
    public TimeSpan Expiration { get; }

    public ExpirationPolicy(TimeSpan expiration)
    {
        Expiration = expiration;
    }
}