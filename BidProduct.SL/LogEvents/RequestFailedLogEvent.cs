using BidProduct.Common;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Extensions;

namespace BidProduct.SL.LogEvents;

public record RequestFailedLogEvent<TRequest, TResponse> : LogEvent where TRequest : IInternalRequest<TResponse>
{
    public string RequestType => typeof(TRequest).GetFullName();
    public TRequest Request { get; set; }
    public int NestingLevel { get; set; }
    public Exception Exception { get; set; }

    public override List<string> Topics { get; set; } = new() { "InternalRequestFailed" };

    public RequestFailedLogEvent(TRequest request, Exception exception)
    {
        Request = request;
        Exception = exception;
    }
}