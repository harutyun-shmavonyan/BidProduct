using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BidProduct.SL.Abstract.CQRS;

namespace BidProduct.Common.LogEvents
{
    public record RequestLogEvent<TRequest, TResponse> : LogEvent where TRequest : IInternalRequest<TResponse>
    {
        public string RequestType => typeof(TRequest).GetFullName();
        public TRequest? Request { get; set; }
        public int NestingLevel { get; set; }
    }
}
