namespace BidProduct.DAL.Abstract.Cache
{
    public interface ICacheValueConverter<TExternalValue, TValue>
    {
        public TValue ConvertToInternalValue(TExternalValue value);
        public TExternalValue? ConvertToExternalValue(TValue value);
    }
}