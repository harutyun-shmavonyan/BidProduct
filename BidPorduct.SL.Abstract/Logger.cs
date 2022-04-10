using BidProduct.Common;
using BidProduct.Common.Abstract;
using Microsoft.Extensions.Logging;

namespace BidProduct.SL.Abstract
{
    public abstract class Logger : ILogger
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly IScopeIdProvider _scopeIdProvider;

        public Logger(IDateTimeService dateTimeService, IScopeIdProvider scopeIdProvider)
        {
            _dateTimeService = dateTimeService;
            _scopeIdProvider = scopeIdProvider;
        }

        public void Log(LogEvent logEvent, LogLevel level)
        {
            var internalLogEvent = new InternalLogEvent
            {
                ScopeId = _scopeIdProvider.ScopeGuid,
                EventDate = _dateTimeService.UtcNow,
                LogLevel = level,
                LogEvent = logEvent
            };

            Log(internalLogEvent);
        }

        abstract protected void Log(InternalLogEvent internalLogEvent);
    }
}