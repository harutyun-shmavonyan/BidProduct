namespace BidProduct.Common.Exceptions
{
    public class ResourceNotFoundException<T, TId> : BidProductException
    {
        public ResourceNotFoundException(TId id) : base($"There is no {typeof(T).Name} with id {id}", ExceptionType.NotFound)
        {
        }
    }
}