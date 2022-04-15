using BidProduct.Common.Abstract;
using BidProduct.SL.Abstract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BidProduct.SL
{
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger(
            IDateTimeService dateTimeService, 
            IScopeIdProvider scopeIdProvider, 
            IUserIdProvider userIdProvider) : base(dateTimeService, scopeIdProvider, userIdProvider)
        {
        }

        protected override void Log(InternalLogEvent internalLogEvent)
        {
            var text = JsonConvert.SerializeObject(internalLogEvent);

            var color = internalLogEvent.LogLevel switch
            {
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Warning => ConsoleColor.DarkYellow,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(text);
        }
    }
}
