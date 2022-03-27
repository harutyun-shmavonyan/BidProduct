using System.Net;
using BidProduct.API.Middlewares;
using BidProduct.Common.Exceptions;

namespace BidProduct.API.ExceptionHandlers
{
    public class NotFoundExceptionHandler : ExceptionHandlerBase
    {
        public override ExceptionResult Execute(BidProductException exception)
        {
            if (exception.ExceptionType == ExceptionType.NotFound)
            {
                var result = new ExceptionResult
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = exception.Message
                };

                return result;
            }

            return Next.Execute(exception);
        }
    }
}