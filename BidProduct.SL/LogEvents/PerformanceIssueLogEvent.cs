using BidProduct.Common;

namespace BidProduct.SL.LogEvents
{
    public record PerformanceIssueLogEvent<TRequest, TResponse> : LogEvent
    {
        public TRequest Request { get; set; }
        public TResponse Response { get; set; }
        public override List<string> Tags{ get; set; } = new() { "InternalRequestPerformanceIssue" };

        public PerformanceIssueLogEvent(TRequest request, TResponse response)
        {
            Request = request;
            Response = response;
        }
    }
}
