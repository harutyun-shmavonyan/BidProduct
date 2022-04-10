using BidProduct.Common;
using Microsoft.Extensions.Logging;

namespace BidProduct.SL.Abstract
{
    public interface ILogger
    {
        void Log(LogEvent logEvent, LogLevel level);
    }
}