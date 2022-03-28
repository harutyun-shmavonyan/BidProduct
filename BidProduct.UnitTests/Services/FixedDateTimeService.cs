using BidProduct.Common.Abstract;
using System;

namespace BidProduct.UnitTests.Services
{
    public class FixedDateTimeService : IDateTimeService
    {
        public DateTimeOffset UtcNow => new DateTimeOffset(2022, 1, 1, 1, 0, 0, 0, TimeSpan.Zero);
    }
}
