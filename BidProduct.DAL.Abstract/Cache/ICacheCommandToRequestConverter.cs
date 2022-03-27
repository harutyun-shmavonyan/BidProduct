namespace BidProduct.DAL.Abstract.Cache
{
    public interface ICacheCommandToRequestConverter<TCommand, TQuery>
    {
        public TQuery Convert(TCommand command);
    }
}