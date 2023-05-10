using BidProduct.SL.Abstract;
using System;

namespace BidProduct.IntegrationTests;

public class TestUserIdProvider : IUserIdProvider
{
    public string UserId => Guid.NewGuid().ToString();
}