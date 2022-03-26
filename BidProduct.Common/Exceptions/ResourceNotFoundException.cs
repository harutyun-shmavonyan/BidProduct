namespace BidProduct.Common.Exceptions
{
    public class ResourceNotFoundException<T> : BidProductException
    {
        public ResourceNotFoundException(long id) : base($"There is no {typeof(T).Name} with id {id}", ExceptionType.NotFound)
        {
        }
        public ResourceNotFoundException(string id) : base($"There is no {typeof(T).Name} with id {id}", ExceptionType.NotFound)
        {
        }
    }
}