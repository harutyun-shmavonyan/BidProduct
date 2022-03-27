using System.Net;
using BidProduct.API.Middlewares;
using BidProduct.Common.Exceptions;

namespace BidProduct.API.ExceptionHandlers;

public class ValidationFailedExceptionHandler : ExceptionHandlerBase
{
    public override ExceptionResult Execute(BidProductException exception)
    {
        if (exception.ExceptionType == ExceptionType.ValidationFailed)
        {
            var result = new ExceptionResult
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = exception.Message
            };

            return result;
        }

        return Next.Execute(exception);
    }
}