namespace BidProduct.SL.Abstract.CQRS
{
    public interface IAnswerSpecificRequest
    {
        public string SessionGuid { get; }
        public long QuestionId { get; }
    }
}