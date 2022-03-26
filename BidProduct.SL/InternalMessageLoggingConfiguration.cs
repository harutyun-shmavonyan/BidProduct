using Microsoft.Extensions.Primitives;

namespace BidProduct.SL
{
    public class InternalMessageLoggingConfiguration
    {
        public bool RequestBodyLoggingNeeded { get; init; }
        public bool ResponseBodyLoggingNeeded { get; init; }
        public bool RequestStartDateNeeded { get; init; }
        public bool ScopeIdNeeded { get; init; }
        public bool DurationNeeded { get; init; }
        public bool ClearDurationNeeded { get; init; }
        public Dictionary<string, int> MaxDurations { get; init; } = (Dictionary<string, int>)Enumerable.Empty<KeyValuePair<string, int>>();
    }
}
