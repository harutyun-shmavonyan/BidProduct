using BidProduct.API.Middlewares;
using BidProduct.Common.Exceptions;

namespace BidProduct.API.ExceptionHandlers
{
    public abstract class ExceptionHandlerBase
    {
        protected ExceptionHandlerBase Next;

        public virtual ExceptionResult Execute(BidProductException exception)
        {
            return new ExceptionResult
            {
                Message = exception.Message
            };
        }

        public void SetSuccessor(ExceptionHandlerBase next) => Next = next;
    }
}