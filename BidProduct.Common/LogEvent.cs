namespace BidProduct.Common;

public abstract record LogEvent
{
    public abstract List<string> Topics { get; set; }

    protected void EnrichWithTopics(params string[] topics) => Topics.AddRange(topics);
}