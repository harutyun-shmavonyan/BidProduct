namespace BidProduct.Common.LogEvents
{
    public record PerformanceIssueLogEvent<TRequest, TResponse> : LogEvent
    {
        public TRequest Request { get; set; }
        public TResponse Response { get; set; }
    }
}
