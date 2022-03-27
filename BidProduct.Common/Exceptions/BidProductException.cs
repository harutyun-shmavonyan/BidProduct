namespace BidProduct.Common.Exceptions
{
    public class BidProductException : Exception
    {
        public ExceptionType ExceptionType { get; }

        public BidProductException(string message, ExceptionType type) : base(message)
        {
            ExceptionType = type;
        }
    }
}
