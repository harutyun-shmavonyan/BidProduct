namespace BidProduct.DAL.Abstract
{
    public interface IHasModified
    {
        DateTimeOffset Modified { get; set; }
    }
}