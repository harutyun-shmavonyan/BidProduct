namespace BidProduct.Common.Abstract;

public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }
}