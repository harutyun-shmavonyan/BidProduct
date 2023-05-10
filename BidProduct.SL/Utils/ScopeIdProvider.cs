using BidProduct.SL.Abstract;

namespace BidProduct.SL.Utils
{
    public class TraceIdProvider : ITraceIdProvider
    {
        public string ScopeGuid { get; }

        public TraceIdProvider(string scopeGuid)
        {
            ScopeGuid = scopeGuid;
        }
    }
}
