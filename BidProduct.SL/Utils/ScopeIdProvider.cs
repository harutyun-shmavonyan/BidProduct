using BidProduct.SL.Abstract;

namespace BidProduct.SL.Utils
{
    public class ScopeIdProvider : IScopeIdProvider
    {
        public string ScopeGuid { get; }

        public ScopeIdProvider(string scopeGuid)
        {
            ScopeGuid = scopeGuid;
        }
    }
}
