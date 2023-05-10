namespace BidProduct.Common;

public abstract record LogEvent
{
    public abstract List<string> Tags { get; set; }

    protected void EnrichWithTopics(params string[] topics) => Tags.AddRange(topics);
}