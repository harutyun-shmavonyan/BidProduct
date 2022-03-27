namespace BidProduct.DAL.Abstract.Cache
{
    public interface ICacheKeyConverter<TInput, TKey>
    {
        public TKey ToKey(TInput input);
    }
}
