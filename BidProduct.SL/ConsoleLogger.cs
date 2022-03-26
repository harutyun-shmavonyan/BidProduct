using Microsoft.Extensions.Logging;

namespace BidProduct.SL
{
    public class ConsoleLogger : Abstract.ILogger
    {
        public void Log(string text, LogLevel logLevel = LogLevel.Information)
        {
            var color = logLevel switch
            {
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Warning => ConsoleColor.DarkYellow,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Log(string topic, string text, LogLevel level = LogLevel.Information)
        {
            throw new NotImplementedException();
        }

        public void Log(ICollection<string> topics, string text, LogLevel level = LogLevel.Information)
        {
            throw new NotImplementedException();
        }
    }
}
