namespace BidProduct.DAL.Abstract
{
    public interface IHasId<TId> where TId : struct
    {
        TId Id { get; set; }
    }
}
