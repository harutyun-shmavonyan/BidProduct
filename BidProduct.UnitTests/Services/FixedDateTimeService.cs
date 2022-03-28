using BidProduct.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BidProduct.UnitTests.Services
{
    public class FixedDateTimeService : IDateTimeService
    {
        public DateTimeOffset UtcNow => new DateTimeOffset(2022, 1, 1, 1, 0, 0, 0, TimeSpan.Zero);
    }
}
