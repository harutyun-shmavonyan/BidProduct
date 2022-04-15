using BidProduct.Common;
using BidProduct.Common.Abstract;
using Microsoft.Extensions.Logging;

namespace BidProduct.SL.Abstract
{
    public abstract class Logger : ILogger
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly IScopeIdProvider _scopeIdProvider;
        private readonly IUserIdProvider _userIdProvider;

        public Logger(IDateTimeService dateTimeService, IScopeIdProvider scopeIdProvider, IUserIdProvider userIdProvider)
        {
            _dateTimeService = dateTimeService;
            _scopeIdProvider = scopeIdProvider;
            _userIdProvider = userIdProvider;
        }

        public void Log(LogEvent logEvent, LogLevel level)
        {
            var internalLogEvent = new InternalLogEvent(logEvent)
            {
                ScopeId = _scopeIdProvider.ScopeGuid,
                UserId = _userIdProvider.UserId,
                EventDate = _dateTimeService.UtcNow,
                LogLevel = level
            };

            Log(internalLogEvent);
        }

        abstract protected void Log(InternalLogEvent internalLogEvent);
    }
}