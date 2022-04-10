using Microsoft.Extensions.Logging;

namespace BidProduct.Common;

public abstract record LogEvent
{
    public List<string> Topics { get; set; } = new();
    public int? UserId { get; set; }
}