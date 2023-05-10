using BidProduct.Common.Abstract;
using BidProduct.SL.Abstract;
using Microsoft.Extensions.Logging;

namespace BidProduct.SL
{
    public class ElasticSearchLogger : Logger
    {
        public ElasticSearchLogger(
            IDateTimeService dateTimeService,
            ITraceIdProvider TraceIdProvider,
            IUserIdProvider userIdProvider) : base(dateTimeService, TraceIdProvider, userIdProvider)
        {
        }

        protected override void Log(InternalLogEvent internalLogEvent)
        {
            switch (internalLogEvent.LogLevel)
            {
                case LogLevel.Trace: Serilog.Log.Verbose("{@internalLogEvent}", internalLogEvent); 
                    break;
                case LogLevel.Debug: Serilog.Log.Debug("{@internalLogEvent}", internalLogEvent);
                    break;
                case LogLevel.Information: Serilog.Log.Information("{@internalLogEvent}", internalLogEvent); 
                    break;
                case LogLevel.Warning: Serilog.Log.Warning("{@internalLogEvent}", internalLogEvent); 
                    break;
                case LogLevel.Error: Serilog.Log.Error("{@internalLogEvent}", internalLogEvent); 
                    break;
                case LogLevel.Critical: Serilog.Log.Fatal("{@internalLogEvent}", internalLogEvent); 
                    break;
            }
        }
    }
}
