using BidProduct.Common;
using BidProduct.Common.Abstract;
using Microsoft.Extensions.Logging;

namespace BidProduct.SL.Abstract
{
    public abstract class Logger : ILogger
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly ITraceIdProvider _TraceIdProvider;
        private readonly IUserIdProvider _userIdProvider;

        public Logger(IDateTimeService dateTimeService, ITraceIdProvider TraceIdProvider, IUserIdProvider userIdProvider)
        {
            _dateTimeService = dateTimeService;
            _TraceIdProvider = TraceIdProvider;
            _userIdProvider = userIdProvider;
        }

        public void Log(LogEvent logEvent, LogLevel level)
        {
            var internalLogEvent = new InternalLogEvent(logEvent)
            {
                TraceId = _TraceIdProvider.ScopeGuid,
                UserId = _userIdProvider.UserId,
                EventDate = _dateTimeService.UtcNow,
                LogLevel = level
            };

            Log(internalLogEvent);
        }

        abstract protected void Log(InternalLogEvent internalLogEvent);
    }
}