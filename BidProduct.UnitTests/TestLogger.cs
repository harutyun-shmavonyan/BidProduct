using BidProduct.Common;
using Microsoft.Extensions.Logging;
using ILogger = BidProduct.SL.Abstract.ILogger;

namespace BidProduct.UnitTests;

public class TestLogger : ILogger
{
    public void Log(LogEvent logEvent, LogLevel level)
    {
    }
}