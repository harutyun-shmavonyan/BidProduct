// ReSharper disable UnusedTypeParameter

using System;

namespace BidProduct.DAL.Abstract.Cache;

public interface IExpirationPolicy<TRequest, TResponse>
{
    public TimeSpan Expiration { get; }
}