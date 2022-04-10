using BidProduct.Common;
using Microsoft.Extensions.Logging;

namespace BidProduct.SL.Abstract
{
    public class InternalLogEvent
    {
        public string ScopeId { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public LogLevel LogLevel { get; set; }
        public LogEvent LogEvent { get; set; }
    }
}