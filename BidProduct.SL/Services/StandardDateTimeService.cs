using BidProduct.Common.Abstract;

namespace BidProduct.SL.Services
{
    public class StandardDateTimeService : IDateTimeService
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
