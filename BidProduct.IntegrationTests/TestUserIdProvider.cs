using System;
using BidProduct.SL.Abstract;

namespace BidProduct.IntegrationTests;

public class TestUserIdProvider : IUserIdProvider
{
    public string UserId => Guid.NewGuid().ToString();
}