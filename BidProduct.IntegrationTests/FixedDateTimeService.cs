using System;
using BidProduct.Common.Abstract;

namespace BidProduct.IntegrationTests;

public class FixedDateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => new DateTimeOffset(2022, 1, 1, 1, 0, 0, 0, TimeSpan.Zero);
}