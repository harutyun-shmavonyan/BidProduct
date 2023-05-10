namespace BidProduct.SL
{
    public class InternalMessageLoggingConfiguration
    {
        public bool RequestBodyLoggingNeeded { get; init; }
        public bool ResponseBodyLoggingNeeded { get; init; }
        public bool TraceIdNeeded { get; init; }
        public bool DurationNeeded { get; init; }
        public bool ClearDurationNeeded { get; init; }
        public Dictionary<string, int>? MaxDurations { get; set; } = new Dictionary<string, int> { { "Default", 1000 } };
    }
}
