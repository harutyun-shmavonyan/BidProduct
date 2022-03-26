namespace BidProduct.DAL.Abstract.Cache
{
    public interface ICacheValueConverter<TExternalValue, TValue>
    {
        public TValue ConvertTo(TExternalValue value);
        public TExternalValue ConvertFrom(TValue value);
    }
}