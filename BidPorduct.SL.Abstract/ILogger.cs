using Microsoft.Extensions.Logging;

namespace BidProduct.SL.Abstract
{
    public interface ILogger
    {
        void Log(string text, LogLevel level = LogLevel.Information);
        void Log(string topic, string text, LogLevel level = LogLevel.Information);
        void Log(ICollection<string> topics, string text, LogLevel level = LogLevel.Information);
    }
}