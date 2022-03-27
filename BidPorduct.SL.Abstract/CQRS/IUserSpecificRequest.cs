namespace BidProduct.SL.Abstract.CQRS
{
    public interface IUserSpecificRequest
    {
        long UserId { get; }
    }
}