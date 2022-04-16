using System.Net;
using BidProduct.API.Middlewares;
using BidProduct.Common.Exceptions;

namespace BidProduct.API.ExceptionHandlers
{
    public class DbUpdateExceptionHandler : ExceptionHandlerBase
    {
        public override ExceptionResult Execute(BidProductException exception)
        {
            if (exception.ExceptionType == ExceptionType.DbUpdateFailed)
            {
                var result = new ExceptionResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = exception.Message
                };

                return result;
            }

            return Next!.Execute(exception);
        }
    }
}