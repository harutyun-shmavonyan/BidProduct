using BidProduct.Common;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Extensions;

namespace BidProduct.SL.LogEvents
{
    public record RequestLogEvent<TRequest, TResponse> : LogEvent where TRequest : IInternalRequest<TResponse>
    {
        public string RequestType => typeof(TRequest).GetFullName();
        public TRequest? Request { get; set; }
        public int NestingLevel { get; set; }

        public override List<string> Tags{ get; set; } = new() { "InternalRequest" };
    }
}
