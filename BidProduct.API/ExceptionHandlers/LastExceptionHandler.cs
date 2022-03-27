using System.Net;
using BidProduct.API.Middlewares;
using BidProduct.Common.Exceptions;

namespace BidProduct.API.ExceptionHandlers
{
    public class LastExceptionHandler : ExceptionHandlerBase
    {
        public override ExceptionResult Execute(BidProductException exception)
        {
            return new ExceptionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"{exception.ExceptionType} wasn't handled"
            };
        }
    }
}