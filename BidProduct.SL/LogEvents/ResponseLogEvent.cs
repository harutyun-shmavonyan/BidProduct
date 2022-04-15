using BidProduct.Common;
using BidProduct.SL.Extensions;

namespace BidProduct.SL.LogEvents
{
    public record ResponseLogEvent<TResponse> : LogEvent
    {
        public string ResponseType => typeof(TResponse).GetFullName();
        public TResponse? Response { get; set; }
        public TimeSpan? ClearDuration { get; set; }
        public TimeSpan? Duration { get; set; }
        public int NestingLevel { get; set; }
        public override List<string> Topics { get; set; } = new() { "InternalResponse" };
    }
}
