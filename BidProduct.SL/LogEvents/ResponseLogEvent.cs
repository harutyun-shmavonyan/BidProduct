namespace BidProduct.Common.LogEvents
{
    public record ResponseLogEvent<TResponse> : LogEvent
    {
        public string ResponseType => typeof(TResponse).GetFullName();
        public TResponse? Response { get; set; }
        public TimeSpan? ClearDuration { get; set; }
        public TimeSpan? Duration { get; set; }
        public int NestingLevel { get; set; }
    }
}
